using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text.Json;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Common.Models.NewFolder;
using Azure;
using Azure.Core;
using GoogleApi.Entities.Interfaces;
using GoogleApi.Entities.Maps.Geocoding;
using GoogleApi.Entities.Search.Common;
using GoogleApi.Entities.Search.Video.Common;
using HtmlAgilityPack;
using MediatR;
using Microsoft.Extensions.Configuration;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoHelper.Infrastructure.Services;

internal class GarageService : IGarageService
{
    private readonly IApplicationDbContext _context;
    private readonly IBlobStorageService _blobStorageService;
    private readonly WebScraperClient _webScraperClient;
    private readonly GoogleApiClient _googleApiClient;
    private readonly RDWApiClient _rdwService;

    public GarageService(
        IApplicationDbContext context,
        IBlobStorageService blobStorageService,
        WebScraperClient webScraperClient,
        GoogleApiClient googleApiClient,
        RDWApiClient rdwService
    ) {
        _context = context;
        _blobStorageService = blobStorageService;
        _webScraperClient = webScraperClient;
        _googleApiClient = googleApiClient;
        _rdwService = rdwService;
    }

    public async Task<IEnumerable<RDWCompany>> GetRDWCompanies(int offset, int limit)
    {
        return await _rdwService.GetAllCompanies(offset, limit);
    }

    public async Task<int> GetRDWCompaniesCount()
    {
        return await _rdwService.GetAllCompaniesCount();
    }

    public async Task<IEnumerable<RDWCompanyService>> GetRDWServices()
    {
        return await _rdwService.GetAllServices();
    }

    public async Task<bool> NeedToUpdate(RDWCompany company, GarageLookupItem? garage)
    {
        // ignore update when garage is using the GarageItem table
        // the user can update the garage information manually
        if (garage?.GarageId != null)
        {
            return false;
        }

        var expectedAddress = GetAddress(company);
        return expectedAddress != garage?.Address;
    }

    public async Task<GarageLookupItem> CreateLookup(RDWCompany company)
    {
        var identifier = company.Volgnummer.ToString();
        var name = company.Naambedrijf.ToPascalCase();
        var address = GetAddress(company);
        var city = company.Plaats.ToPascalCase();

        var garage = new GarageLookupItem()
        {
            Identifier = identifier,
            Name = name,
            Address = address,
            City = city,
            CreatedBy = $"System:{nameof(CreateLookup)}",
            Created = DateTime.UtcNow,
            LastModifiedBy = $"System:{nameof(CreateLookup)}",
            LastModified = DateTime.UtcNow,
        };

        garage = await SetInformationFromGoogle(garage);
        garage = await SetInformationFromWebScraper(garage);
        return garage;
    }

    public async Task<GarageLookupItem> UpdateLookup(RDWCompany company, GarageLookupItem garage)
    {
        var name = company.Naambedrijf.ToPascalCase();
        var address = GetAddress(company);
        var city = company.Plaats.ToPascalCase();

        garage.Name = name;
        garage.Address = address;
        garage.City = city;

        garage = await SetInformationFromGoogle(garage);
        garage = await SetInformationFromWebScraper(garage);
        return garage;
    }

    public async Task<(
        List<GarageLookupServiceItem> itemsToInsert, 
        List<GarageLookupServiceItem> itemsToUpdate,
        List<GarageLookupServiceItem> itemsToRemove
    )>  
    UpsertLookupServices(
        IEnumerable<GarageLookupServiceItem>? garageServices, 
        IEnumerable<GarageLookupServiceItem> rdwServices, 
        string garageIdentifier
    )
    {
        var itemsToInsert = new List<GarageLookupServiceItem>();
        var itemsToUpdate = new List<GarageLookupServiceItem>();
        foreach (var rdwService in rdwServices)
        {
            var garageService = garageServices?.FirstOrDefault(x => 
                x.GarageLookupIdentifier == garageIdentifier &&
                x.Type == rdwService.Type &&
                x.VehicleType == rdwService.VehicleType && 
                x.Title == rdwService.Title
            );

            if (garageService == null)
            {
                var service = await CreateService(garageIdentifier, rdwService);
                itemsToInsert.Add(service);
            }
            else
            {
                var needToUpdate = NeedToUpdate(garageService!, rdwService!);
                if (needToUpdate)
                {
                    var service = await UpdateService(garageService, rdwService);
                    itemsToInsert.Add(service);
                    itemsToUpdate.Add(garageService);
                }
            }
        }

        var itemsToRemove = new List<GarageLookupServiceItem>();
        if (garageServices != null)
        {
            foreach (var garageService in garageServices)
            {
                var rdwService = rdwServices.FirstOrDefault(x =>
                    x.GarageLookupIdentifier == garageIdentifier &&
                    x.Type == garageService.Type && 
                    x.VehicleType == garageService.VehicleType && 
                    x.Title == garageService.Title
                );

                if (rdwService == null)
                {
                    itemsToRemove.Add(garageService);
                }
            }
        }

        return (itemsToInsert, itemsToUpdate, itemsToRemove);
    }

    private static bool NeedToUpdate(GarageLookupServiceItem garageService, GarageLookupServiceItem rdwService)
    {
        if (garageService.Description != rdwService.Description)
        {
            return true;
        }

        if (garageService.ExpectedNextDateIsRequired != rdwService.ExpectedNextDateIsRequired)
        {
            return true;
        }

        if (garageService.ExpectedNextOdometerReadingIsRequired != rdwService.ExpectedNextOdometerReadingIsRequired)
        {
            return true;
        }

        return false;
    }

    public async Task<GarageLookupServiceItem> CreateService(string garageLookupIdentifier, GarageLookupServiceItem rdwService)
    {
        var service = new GarageLookupServiceItem()
        {
            Id = Guid.NewGuid(),
            GarageLookupIdentifier = garageLookupIdentifier,
            Type = rdwService.Type,
            VehicleType = rdwService.VehicleType,
            Title = rdwService.Title,
            Description = rdwService.Description,
            ExpectedNextDateIsRequired = rdwService.ExpectedNextDateIsRequired,
            ExpectedNextOdometerReadingIsRequired = rdwService.ExpectedNextOdometerReadingIsRequired,
        };

        return service;
    }

    public async Task<GarageLookupServiceItem> UpdateService(GarageLookupServiceItem service, GarageLookupServiceItem rdwService)
    {
        service.Description = rdwService.Description;
        service.ExpectedNextDateIsRequired = rdwService.ExpectedNextDateIsRequired;
        service.ExpectedNextOdometerReadingIsRequired = rdwService.ExpectedNextOdometerReadingIsRequired;
        return service;
    }

    private static string GetAddress(RDWCompany company)
    {
        var street = company.Straat;
        var houseNumber = company.Huisnummer.ToString();
        var houseNumberAddition = company.Huisnummertoevoeging;

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

    private async Task<GarageLookupItem> SetInformationFromGoogle(GarageLookupItem item)
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
        var reference = details.result.photos?[0].photo_reference;
        if (!string.IsNullOrEmpty(reference))
        {
            var (fileBytes, fileExtension) = await _googleApiClient.GetPlacePhoto(reference, 1000);
            if (fileBytes != null)
            {
                // Upload original image
                item.Image = await _blobStorageService.UploadGarageImageAsync(fileBytes, fileExtension, CancellationToken.None);

                // Create and upload thumbnail image
                var thumbnailBytes = CreateThumbnail(fileBytes, 150);
                item.ImageThumbnail = await _blobStorageService.UploadGarageImageAsync(thumbnailBytes, fileExtension, CancellationToken.None);
            }
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

        // TODO: Get opening hours from website

        return item;
    }

    private static byte[] CreateThumbnail(byte[] originalImage, int thumbnailHeight)
    {
        using var image = Image.Load(originalImage);
        // Calculate the new width while maintaining the aspect ratio
        int newWidth = (int)((double)image.Width / ((double)image.Height / (double)thumbnailHeight));

        image.Mutate(x => x.Resize(newWidth, thumbnailHeight));
        using var memoryStream = new MemoryStream();
        image.SaveAsJpeg(memoryStream);
        return memoryStream.ToArray();
    }

    private async Task<GarageLookupItem> SetInformationFromWebScraper(GarageLookupItem item)
    {
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

        // TODO: Validate Email* and phone numbers that really has valid value

        return item;
    }
}
