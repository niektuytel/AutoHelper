using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
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

internal class GarageService : IGarageService
{
    private readonly WebScraperClient _webScraperClient;
    private readonly GoogleApiClient _googleApiClient;
    private readonly RDWApiClient _rdwApiClient;

    public GarageService(
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
        var garageLookups = new List<GarageLookupItem>();

        foreach (var rdwCompany in rdwCompanies)
        {
            var services = rdwServices
                .Where(y => y.Volgnummer == rdwCompany.Volgnummer)
                .Select(y => y.ServiceType)
                .ToArray();

            // Has no services to offer, invalid garage
            if (
                rdwCompany.Volgnummer == 0 ||
                services.Length == 0 ||
                string.IsNullOrWhiteSpace(rdwCompany.Naambedrijf) ||
                string.IsNullOrWhiteSpace(rdwCompany.Plaats) ||
                string.IsNullOrWhiteSpace(rdwCompany.Straat)
            ) {
                continue;
            }

            var garageLookup = new GarageLookupItem
            {
                Identifier = rdwCompany.Volgnummer.ToString(),
                Name = rdwCompany.Naambedrijf,
                KnownServices = services,
                Address = FormatAddress(rdwCompany.Straat, rdwCompany.Huisnummer.ToString(), rdwCompany.Huisnummertoevoeging),
                City = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(rdwCompany.Plaats.ToLower())
            };

            garageLookups.Add(garageLookup);
        }

        return garageLookups.ToArray();
    }

    private static string FormatAddress(string street, string houseNumber, string? houseNumberAddition)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new Exception("Street is empty");
        }

        if (string.IsNullOrWhiteSpace(houseNumber))
        {
            throw new Exception("House number is empty");
        }

        // Capitalize the first letter of the street
        street = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(street.ToLower());

        // Remove unexpected commas from the inputs
        street = street.Replace(",", "").Trim();
        houseNumber = houseNumber.Replace(",", "").Trim();
        houseNumberAddition = string.IsNullOrWhiteSpace(houseNumberAddition) ? "" : houseNumberAddition.Replace(",", "").Trim();

        // Conditionally add comma based on the presence of houseNumber
        if (!string.IsNullOrEmpty(houseNumber))
        {
            return $"{street} {houseNumber}{houseNumberAddition}";
        }
        else
        {
            return $"{street} {houseNumberAddition}".Trim();
        }
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

        // TODO: Set small first photo (if exist)
        var reference = details.result.photos?[0].photo_reference;
        if (!string.IsNullOrEmpty(reference))
        {
            var photo = await _googleApiClient.GetPlacePhotoInBase64(reference, 2000);
            item.LargeData.FirstPlacePhoto = photo;
        }

        item.Status = details.result.business_status;
        item.DaysOfWeek = details.result.opening_hours?.periods != null ?
            details.result.opening_hours?.periods!.Select(x => x.open.day).Distinct().ToArray()
            :
            null;
        item.PhoneNumber = details.result.formatted_phone_number;

        if (details.result.geometry.location != null)
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

        // TODO: Get opening hours from website
        // TODO: store photos in blob and store url inside here
        // TODO: Validate Email* and phone numbers that really has valid value

        return item;
    }

    // TODO: Need better investigation
    public IEnumerable<GarageServiceType> GetRelatedServiceTypes(VehicleType vehicleType)
    {
        return vehicleType switch
        {
            VehicleType.LightCar => new List<GarageServiceType>()
            {
                GarageServiceType.MOTServiceLightVehicle,
                GarageServiceType.SmallMaintenance,
                GarageServiceType.GreatMaintenance,
                GarageServiceType.SeasonalTireChange,
                GarageServiceType.AirConditioningMaintenance,
                GarageServiceType.Inspection,
                GarageServiceType.AcceleratedRegistrationService
            },
            _ => new List<GarageServiceType>(),
        };
    }
}
