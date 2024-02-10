using System.Text.Json;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Messaging.Models;
using AutoHelper.Messaging.Models.GraphEmail;
using AutoHelper.Messaging.Templates.Conversation;
using AutoHelper.Messaging.Templates.Notification;
using BlazorTemplater;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

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

    public async Task SendMessage(ConversationMessageItem message, string senderName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        // TODO: build html, so it looks like 1 conversation.
        // now when send an message from Whatsapp to the email
        // he will send an clean message, this will been handled in gmail as 1 message.
        // but we want an tree of responses that it looks like an conversation in gmail

        string html = new ComponentRenderer<Templates.Conversation.Message>()
            .Set(c => c.Content, content)
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

    public async Task SendMessageWithVehicle(ConversationMessageItem message, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.ReceiverContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

        string html = new ComponentRenderer<MessageWithVehicle>()
            .Set(c => c.LicensePlate, vehicle.LicensePlate)
            .Set(c => c.Content, content)
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

    public async Task SendMessageConfirmation(ConversationMessageItem message, string receiverName, CancellationToken cancellationToken)
    {
        var receiverIdentifier = message.SenderContactIdentifier;
        var conversationId = message.ConversationId;
        var content = message.MessageContent;

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
                            Address = receiverIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    public async Task SendNotificationMessage(NotificationItem notification, CancellationToken cancellationToken)
    {
        switch (notification.GeneralType)
        {
            case NotificationGeneralType.GarageServiceReviewReminder:
                await SendGarageServiceReviewReminder(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewApproved:
                await SendVehicleServiceReviewApproved(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceReviewDeclined:
                await SendVehicleServiceReviewDeclined(notification, cancellationToken);
                break;
            case NotificationGeneralType.VehicleServiceNotification:
                await SendVehicleServiceNotification(notification, cancellationToken);
                break;
        }
    }

    private async Task SendGarageServiceReviewReminder(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<GarageServiceReviewReminder>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Bevestiging gevraagd voor onderhoud aan ${notification.VehicleLicensePlate}",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    private async Task SendVehicleServiceReviewApproved(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewApproved>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Onderhoudsregel Goedgekeurd voor [{notification.VehicleLicensePlate}]: Bevestiging van Garage",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    private async Task SendVehicleServiceReviewDeclined(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = new ComponentRenderer<VehicleServiceReviewDeclined>()
            .Set(c => c.Notification, notification)
            .Render();

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = $"Onderhoudsregel afgekeurd voor [{notification.VehicleLicensePlate}]: Bevestiging van Garage",
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
                            Address = notification.ReceiverContactIdentifier
                        }
                    }
                }
            }
        };

        await SendEmail(email);
    }

    private async Task SendVehicleServiceNotification(NotificationItem notification, CancellationToken cancellationToken)
    {
        string html = "";
        string subject = "";
        switch (notification.VehicleType)
        {
            case NotificationVehicleType.MOT:
                subject = $"APK verloopt over 4 weken voor [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_MOT>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.WinterService:
                subject = $"Zorg goed voor uw auto: Overweeg een onderhoudsbeurt na een intensieve winterperiode [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_WinterService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToSummerTyre:
                subject = $"Tijd om uw Winterbanden te Wisselen voor de Zomer: [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerTyreChange>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerCheck:
                subject = $"Is uw auto klaar voor de vakantie? Plan een Zomercheck voor [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerCheck>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.SummerService:
                subject = $"Heeft u een vakantietrip gemaakt? Overweeg een onderhoudsbeurt voor uw auto [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_SummerService>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
            case NotificationVehicleType.ChangeToWinterTyre:
                subject = $"Bereid uw auto voor op de winter: Tijd voor winterbanden [{notification.VehicleLicensePlate}]";
                html = new ComponentRenderer<VehicleServiceNotification_WinterTyreChange>()
                    .Set(c => c.Notification, notification)
                    .Render();
                break;
        }

        var email = new GraphEmail
        {
            Message = new GraphEmailMessage
            {
                Subject = subject,
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
                            Address = notification.ReceiverContactIdentifier
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

    public Task SendNotificationMessage(NotificationItem notification, VehicleTechnicalDtoItem vehicle, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
