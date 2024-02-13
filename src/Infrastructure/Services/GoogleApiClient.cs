using System.Text.Json;
using AutoHelper.Infrastructure.Common.Models;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AutoHelper.Infrastructure.Services;

internal class GoogleApiClient : IGoogleApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GoogleApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["Google:Api_Key"] ?? throw new Exception("Google:Api_Key is not set");
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/search-find-place
    /// </summary>
    /// <param name="queryText">{name} in {address}, {city}</param>
    /// <returns>place_id related to garage</returns>
    public async Task<string?> SearchPlaceIdFromTextQuery(string queryText)
    {
        try
        {
            var url = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json" +
                "?fields=place_id" +
                "&language=nl" +
                $"&input={queryText}" +
                "&inputtype=textquery" +
                $"&key={_apiKey}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var placeItem = JsonSerializer.Deserialize<GoogleApiSearchPlaceItem>(content);
                return placeItem?.candidates[0]?.place_id;
            }
        }
        catch (Exception ex)
        {
            // continue as there is no place_id been founded
        }

        return null;
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/details
    /// </summary>
    /// <returns>all details related to this place</returns>
    public async Task<GoogleApiDetailPlaceItem?> GetPlaceDetailsFromPlaceId(string place_id)
    {
        var url = "https://maps.googleapis.com/maps/api/place/details/json" +
            "?fields=name,place_id,business_status,editorial_summary,formatted_phone_number,geometry,icon,icon_background_color,icon_mask_base_uri,opening_hours,photos,price_level,rating,reviews,secondary_opening_hours,user_ratings_total,website" +
            "&language=nl" +
            $"&place_id={place_id}" +
            $"&key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var details = JsonSerializer.Deserialize<GoogleApiDetailPlaceItem>(content)!;
            return details;
        }

        return null;
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/photos
    /// </summary>
    public async Task<(byte[]? fileBytes, string fileExtension)> GetPlacePhoto(string photo_reference, int maxWidth)
    {
        var url = "https://maps.googleapis.com/maps/api/place/photo" +
            $"?maxwidth={maxWidth}" +
            $"&photo_reference={photo_reference}" +
            $"&key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var contentType = response.Content.Headers.ContentType?.MediaType;

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var fileExtension = GetFileExtension(contentType);
            return (bytes, fileExtension);
        }

        return (null, "");
    }

    public byte[] CreateThumbnail(byte[] originalImage, int thumbnailHeight)
    {
        using var image = Image.Load(originalImage);
        // Calculate the new width while maintaining the aspect ratio
        int newWidth = (int)((double)image.Width / ((double)image.Height / (double)thumbnailHeight));

        image.Mutate(x => x.Resize(newWidth, thumbnailHeight));
        using var memoryStream = new MemoryStream();
        image.SaveAsJpeg(memoryStream);
        return memoryStream.ToArray();
    }

    private string GetFileExtension(string? contentType)
    {
        switch (contentType)
        {
            case "image/jpeg":
                return ".jpg";
            case "image/png":
                return ".png";
            case "image/bmp":
                return ".bmp";
            case "image/gif":
                return ".gif";
            case "image/tiff":
                return ".tiff";
            case "image/svg+xml":
                return ".svg";
            case "image/webp":
                return ".webp";
            case "image/heif":
                return ".heif";
            case "image/heic":
                return ".heic";
            default:
                return "";
        }
    }

}
