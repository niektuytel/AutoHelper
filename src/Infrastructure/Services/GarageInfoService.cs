using System;
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
    public int CalculateDistanceInKm(GarageLocationItem location, float latitude, float longitude)
    {
        var preciseDistance = LocationExtentions.CalculateDistance(location.Latitude, location.Longitude, latitude, longitude);
        return (int)Math.Round(preciseDistance);
    }
}
