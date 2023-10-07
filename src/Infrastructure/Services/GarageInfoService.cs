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
using AutoHelper.Infrastructure.Common.Models.NewFolder;
using Azure;
using Azure.Core;
using GoogleApi.Entities.Maps.Geocoding;
using GoogleApi.Entities.Search.Common;
using GoogleApi.Entities.Search.Video.Common;
using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class GarageInfoService : IGarageInfoService
{
    private readonly WebScraperClient _webScraperClient;
    private readonly GoogleApiClient _googleApiClient;
    private readonly RDWApiClient _rdwApiClient;

    public GarageInfoService(
        WebScraperClient webScraperClient,
        GoogleApiClient googleApiClient, 
        RDWApiClient rdwApiClient
    ) {
        _webScraperClient = webScraperClient;
        _googleApiClient = googleApiClient;
        _rdwApiClient = rdwApiClient;
    }

    public async Task<GarageLookupItem[]> GetBriefGarageLookups()
    {
        var rdwCompanies = await _rdwApiClient.GetKnownCompanies();
        var rdwServices = await _rdwApiClient.GetKnownServices();

        var garageLookups = rdwCompanies
            .Select(x => new GarageLookupItem
            {
                Identifier = x.Volgnummer.ToString(),
                Name = x.Naambedrijf,
                KnownServices = rdwServices
                    .Where(y => y.Volgnummer == x.Volgnummer)
                    .Select(y => y.Erkenning)
                    .ToArray(),
                Address = $"{x.Straat}, {x.Huisnummer}{x.Huisnummertoevoeging}",
                City = x.Plaats
            })
            .ToArray();

        return garageLookups;
    }

    public int CalculateDistanceInKm(float garageLatitude, float garageLongitude, float latitude, float longitude)
    {
        var preciseDistance = LocationExtentions.CalculateDistance(garageLatitude, garageLongitude, latitude, longitude);
        return (int)Math.Round(preciseDistance);
    }

    public async Task<GarageLookupItem> UpdateByAddressAndCity(GarageLookupItem item)
    {
        // Get place id from the given name, address and city
        var placeId = await _googleApiClient.SearchPlaceIdFromTextQuery($"{item.Name} in {item.Address}, {item.City}");
        if (placeId == null)
        {
            return item;
        }

        // Get details from the given place id
        var placeDetailsJson = await _googleApiClient.GetPlaceDetailsFromPlaceId(placeId);
        if (placeDetailsJson == null)
        {
            throw new Exception("Error fetching data from Google Maps API");
        }

        var details = JsonSerializer.Deserialize<GoogleApiDetailPlaceItem>(placeDetailsJson)!;
        item.LargeData = new GarageLookupLargeItem()
        {
            Id = item.LargeData == null ? Guid.NewGuid() : item.LargeData.Id, 
            GoogleApiDetailsJson = placeDetailsJson
        };

        // Set small first photo (if exist)
        var reference = details.result.photos?[0].photo_reference;
        if(!string.IsNullOrEmpty(reference))
        {
            var photo = await _googleApiClient.GetPlacePhotoInBase64(reference, 400);
            item.FirstPlacePhoto = photo;
        }

        item.Status = details.result.business_status;
        item.DaysOfWeek = details.result.opening_hours?.periods != null ?
            details.result.opening_hours?.periods!.Select(x => x.open.day).ToArray()
            :
            null;
        item.PhoneNumber = details.result.formatted_phone_number;

        if(details.result.geometry.location != null)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            item.Location = geometryFactory.CreatePoint(new Coordinate(details.result.geometry.location.lng, details.result.geometry.location.lat));
        }

        item.Website = details.result.website;
        item.Rating = details.result.rating;
        item.UserRatingsTotal = details.result.user_ratings_total;

        try
        {
            // data scraped from website
            if (!string.IsNullOrEmpty(item.Website))
            {
                if (string.IsNullOrEmpty(item.PhoneNumber))
                {
                    item.PhoneNumber = await _webScraperClient.GetPhoneNumberAsync(item.Website);
                }

                if (string.IsNullOrEmpty(item.EmailAddress))
                {
                    item.EmailAddress = await _webScraperClient.GetEmailAddressAsync(item.Website);
                }

                if (string.IsNullOrEmpty(item.WhatsappNumber))
                {
                    item.WhatsappNumber = await _webScraperClient.GetWhatsappNumberAsync(item.Website);
                }
            }
        }
        catch (Exception)
        {
            // ignore errors, website is not always available
        }

        return item;
    }
}
