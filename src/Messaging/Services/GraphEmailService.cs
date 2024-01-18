using AutoHelper.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoHelper.Application.Conversations._DTOs;
using System.Text.Json;
using AutoHelper.Messaging.Models;
using Microsoft.Extensions.Caching.Memory;
using AutoHelper.Messaging.Models.GraphEmail;
using BlazorTemplater;
using AutoHelper.Messaging.Templates;
using WhatsappBusiness.CloudApi.Response;
using RazorEngine.Text;

namespace AutoHelper.Messaging.Services;

/// <summary>
/// https://learn.microsoft.com/en-us/graph/api/resources/message?view=graph-rest-1.0#methods
/// </summary>
internal class GraphEmailService : IMailingService
{
    private readonly SemaphoreSlim _tokenRefreshSemaphore = new SemaphoreSlim(1, 1);
    private const int ExpirationBufferTime = 60;
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMilliseconds = 1000;

    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    private readonly bool _isDevelopment;
    private readonly string _userId;
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _testEmailAddress;

    public GraphEmailService(IMemoryCache memoryCache, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClientFactory.CreateClient(nameof(GraphEmailService));

        _isDevelopment = _configuration["Environment"] == "Development";
        _userId = _configuration["GraphMicrosoft:UserId"]!;
        _tenantId = _configuration["GraphMicrosoft:TenantId"]!;
        _clientId = _configuration["GraphMicrosoft:ClientId"]!;
        _clientSecret = _configuration["GraphMicrosoft:ClientSecret"]!;
        _testEmailAddress = _configuration["GraphMicrosoft:TestEmailAddress"]!;
    }

    public async Task SendMessageRaw(string receiverIdentifier, Guid conversationId, string senderName, string message)
    {
        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Een bericht van {senderName}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = message
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _userId
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    public async Task SendMessage(string receiverIdentifier, Guid conversationId, string senderName, string message)
    {
        string html = new ComponentRenderer<Templates.Message>()
            .Set(c => c.Content, message)
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Een bericht van {senderName}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _userId
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    public async Task SendMessageWithVehicle(string receiverIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicle, string message)
    {
        string html = new ComponentRenderer<MessageWithVehicle>()
            .Set(c => c.LicensePlate, vehicle.LicensePlate)
            .Set(c => c.Content, message)
            .Set(c => c.FuelType, vehicle.FuelType)
            .Set(c => c.Model, $"{vehicle.Brand} {vehicle.Model}({vehicle.YearOfFirstAdmission})")// Dacia Sandero (2008)
            .Set(c => c.MOT, vehicle.MOTExpiryDate)
            .Set(c => c.NAP, vehicle.Mileage)
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Een vraag namens {vehicle.LicensePlate}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _userId
                    }
                },
                ToRecipients = new GraphEmailRecipient[] 
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress 
                        { 
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    public async Task SendMessageConfirmation(string senderIdentifier, Guid conversationId, string receiverName)
    {
        string html = new ComponentRenderer<MessageConfirmation>()
            .Set(c => c.ConversationId, conversationId.ToString().Split('-')[0])
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Bericht is verstuurd naar {receiverName}",
                Body = new GraphEmailBody
                {
                    ContentType = "HTML",
                    Content = html
                },
                From = new GraphEmailFrom
                {
                    EmailAddress = new GraphEmailAddress
                    {
                        Name = "AutoHelper",
                        Address = _userId
                    }
                },
                ToRecipients = new GraphEmailRecipient[]
                {
                    new GraphEmailRecipient()
                    {
                        EmailAddress = new GraphEmailAddress
                        {
                            Address = senderIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    private async Task<string> GetAccessToken()
    {
        const string cacheKey = "GraphAccessToken";
        if (_memoryCache.TryGetValue(cacheKey, out string? accessToken) && !string.IsNullOrEmpty(accessToken))
        {
            return accessToken;
        }

        await _tokenRefreshSemaphore.WaitAsync();
        try
        {
            // Recheck cache after acquiring the semaphore
            if (_memoryCache.TryGetValue(cacheKey, out accessToken) && !string.IsNullOrEmpty(accessToken))
            {
                return accessToken;
            }

            var token = await FetchNewAccessToken();
            var expirationTime = DateTime.UtcNow.AddSeconds(token.ExpiresIn - ExpirationBufferTime);
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expirationTime
            };

            _memoryCache.Set(cacheKey, token.AccessToken, cacheEntryOptions);
            return token.AccessToken;
        }
        finally
        {
            _tokenRefreshSemaphore.Release();
        }
    }

    private async Task<GraphAccessToken> FetchNewAccessToken()
    {
        int retryCount = 0;
        while (retryCount < MaxRetryAttempts)
        {
            try
            {
                var tokenEndpoint = $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0/token";
                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["scope"] = "https://graph.microsoft.com/.default",
                    ["client_id"] = _clientId,
                    ["client_secret"] = _clientSecret,
                    ["grant_type"] = "client_credentials"
                });

                var response = await _httpClient.PostAsync(tokenEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"Unable to retrieve access token: {responseString}");
                }

                var result = JsonSerializer.Deserialize<GraphAccessToken>(responseString);
                return result!;
            }
            catch (Exception ex) when (ex is HttpRequestException)
            {
                retryCount++;
                if (retryCount >= MaxRetryAttempts) throw;
                await Task.Delay(RetryDelayMilliseconds);
            }
        }

        throw new ApplicationException("Failed to retrieve access token after maximum retry attempts.");
    }

    private async Task SendEmail(GraphEmail email)
    {
        var accessToken = await GetAccessToken();
        var emailJson = JsonSerializer.Serialize(email);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"https://graph.microsoft.com/v1.0/users/{_userId}/sendMail");
        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        requestMessage.Content = new StringContent(emailJson, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(requestMessage);
        var responseString = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new ApplicationException($"Failed to send email: {responseString}");
        }
    }

}
