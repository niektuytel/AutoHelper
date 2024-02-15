using System.Text.Json;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Messaging.Interfaces;
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
internal class EmailService : IEmailService
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

    public EmailService(IMemoryCache memoryCache, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClientFactory.CreateClient(nameof(EmailService));

        _isDevelopment = _configuration["Environment"] == "Development";
        _userId = _configuration["GraphMicrosoft:UserId"]!;
        _tenantId = _configuration["GraphMicrosoft:TenantId"]!;
        _clientId = _configuration["GraphMicrosoft:ClientId"]!;
        _clientSecret = _configuration["GraphMicrosoft:ClientSecret"]!;
        _testEmailAddress = _configuration["GraphMicrosoft:TestEmailAddress"]!;
    }

    public async Task SendEmail(GraphEmail email)
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
    
    public string GetUserId() 
    { 
        return _userId;
    }

    public string GetTestEmailAddress()
    {
        return _testEmailAddress;
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
}
