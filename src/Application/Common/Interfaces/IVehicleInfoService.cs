using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleInfoService
{
    Task<bool> ValidVehicle(string licensePlate);
    Task<VehicleBriefInfoItemDto?> GetVehicleBriefInfo(string licensePlate);
    Task<VehicleInfoItemDto> GetVehicleInfoQuery(string licensePlate);
    Task<VehicleType> GetVehicleType(string licensePlate);
}