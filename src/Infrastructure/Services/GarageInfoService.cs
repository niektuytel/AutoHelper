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
using AutoHelper.Infrastructure.Common.Models.NewFolder;
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

internal class GarageInfoService : IGarageInfoService
{
    private readonly RDWService _rdwService;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GarageInfoService(RDWService rdwService, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _rdwService = rdwService;
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["Google:Api_Key"] ?? throw new Exception("Google:Api_Key is not set");
    }

    public async Task<IEnumerable<GarageLookupItem>> GetBriefGarageLookups()
    {
        var rdwCompanies = await _rdwService.GetRDWRecognizedCompanies();
        var garageLookups = rdwCompanies.Select(x => new GarageLookupItem
        {
            Identifier = x.Volgnummer.ToString(),
            Name = x.Naambedrijf,
            Address = $"{x.Straat}, {x.Huisnummer}{x.Huisnummertoevoeging}",
            City = x.Plaats
        }).ToArray();

        return garageLookups;
    }

    public int CalculateDistanceInKm(float garageLatitude, float garageLongitude, float latitude, float longitude)
    {
        var preciseDistance = LocationExtentions.CalculateDistance(garageLatitude, garageLongitude, latitude, longitude);
        return (int)Math.Round(preciseDistance);
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/search-find-place
    /// </summary>
    /// <param name="queryText">{name} in {address}, {city}</param>
    /// <returns>place_id related to garage</returns>
    private async Task<string?> GetGooglePlaceIdFromQuery(string queryText)
    {
        var url = "https://maps.googleapis.com/maps/api/place/findplacefromtext/json" +
            "?fields=place_id" +
            "&language=nl" +
            $"&input=Autoservice Embrechts in Ohmweg 37, Alblasserdam" + //"{queryText}" +
            "&inputtype=textquery" +
            $"&key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        var placeItem = JsonSerializer.Deserialize<GoogleApiSearchPlaceItem>(content);

        return placeItem?.candidates[0]?.place_id;
    }

    /// <summary>
    /// https://developers.google.com/maps/documentation/places/web-service/details
    /// </summary>
    /// <returns>all details related to this place</returns>
    private async Task<string?> GetGooglePlaceDetails(string place_id)
    {
        var url = "https://maps.googleapis.com/maps/api/place/details/json" +
            "?fields=name,place_id,business_status,editorial_summary,formatted_phone_number,geometry,icon,icon_background_color,icon_mask_base_uri,opening_hours,photos,price_level,rating,reviews,secondary_opening_hours,user_ratings_total,website" +
            "&language=nl" +
            $"&place_id={place_id}" +
            $"&key={_apiKey}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    public async Task<GarageLookupItem> UpdateByAddressAndCity(GarageLookupItem item)
    {
        if(string.IsNullOrWhiteSpace(item.City) || string.IsNullOrWhiteSpace(item.Address) || string.IsNullOrWhiteSpace(item.Name))
        {
            throw new InvalidDataException("City, Address and Name are required");
        }

        // Get place id from the given name, address and city
        var placeId = await GetGooglePlaceIdFromQuery($"{item.Name} in {item.Address}, {item.City}");
        if (placeId == null)
        {
            throw new NotFoundException("No garage found");
        }

        // Get details from the given place id
        var placeDetailsJson = await GetGooglePlaceDetails(placeId);
        if (placeDetailsJson == null)
        {
            throw new Exception("Error fetching data from Google Maps API");
        }

        var details = JsonSerializer.Deserialize<GoogleApiDetailPlaceItem>(placeDetailsJson)!;
        item.GoogleApiDetailsJson = placeDetailsJson;
        item.Status = details.result.business_status;
        item.ImageUrl = details.result.photos?[0].photo_reference != null ? $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photo_reference={details.result.photos?[0].photo_reference}&key={_apiKey}" : null;
        item.DaysOfWeek = details.result.opening_hours?.periods != null ?
            details.result.opening_hours?.periods!.Select(x => x.open.day).ToArray()
            :
            null;
        item.PhoneNumber = details.result.formatted_phone_number;
        item.Latitude = details.result.geometry.location.lat;
        item.Longitude = details.result.geometry.location.lng;
        item.Website = details.result.website;
        item.Rating = details.result.rating;
        item.UserRatingsTotal = details.result.user_ratings_total;


        // TODO: Make an scraper to get the phone number, email address and whatsapp number
        if(!string.IsNullOrEmpty(item.Website))
        {
            var scraper = new HtmlWeb();
            var doc = await scraper.LoadFromWebAsync(item.Website);
            var phoneNumber = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'tel')]");
            var emailAddress = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'mailto')]");
            var whatsappNumber = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'whatsapp')]");

            item.PhoneNumber = phoneNumber?.InnerText;
            item.EmailAddress = emailAddress?.InnerText;
            item.WhatsappNumber = whatsappNumber?.InnerText;
        }


        throw new NotImplementedException();
    }
}
