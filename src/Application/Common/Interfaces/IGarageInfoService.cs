using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using MediatR;

namespace AutoHelper.Application.Common.Interfaces;

public interface IGarageInfoService
{
    int CalculateDistanceInKm(GarageLocationItem location, float latitude, float longitude);
}