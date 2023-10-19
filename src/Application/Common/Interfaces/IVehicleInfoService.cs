using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleInfoService
{
    Task<bool> ValidVehicle(string licensePlate);
    Task<VehicleBriefDtoItem?> GetVehicleBriefInfo(string licensePlate);
    Task<VehicleSpecsDtoItem> GetVehicleInfoQuery(string licensePlate);
    Task<VehicleType> GetVehicleType(string licensePlate);
}