using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Common.Models;
using Azure;
using Azure.Core;
using GoogleApi.Entities.Maps.Geocoding;
using GoogleApi.Entities.Search.Common;
using GoogleApi.Entities.Search.Video.Common;
using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class GoogleApiClient
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
    public async Task<string?> GetPlaceDetailsFromPlaceId(string place_id)
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
            return content;
        }

        return null;
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/photos
    /// </summary>
    public async Task<string?> GetPlacePhotoInBase64(string photo_reference, int maxWidth)
    {
        var url = "https://maps.googleapis.com/maps/api/place/photo" +
            $"?maxwidth={maxWidth}" +
            $"&photo_reference={photo_reference}" +
            $"&key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64String = Convert.ToBase64String(bytes);

            return base64String;
        }

        return null;
    }

}
