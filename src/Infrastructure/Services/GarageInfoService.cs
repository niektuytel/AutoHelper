using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Infrastructure.Common.Extentions;
using Azure;
using Azure.Core;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class GarageInfoService : IGarageInfoService
{
    private readonly RDWService _rdwService;

    public GarageInfoService(RDWService rdwService)
    {
        _rdwService = rdwService;
    }

    public async Task<IEnumerable<GarageLookupItem>> GetRDWGarageLookups()
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

}
