using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleService
{
    Task<bool> ValidVehicle(string licensePlate);
    Task<VehicleBriefInfoItemDto?> GetVehicleBriefInfo(string licensePlate);
    Task<VehicleInfoItemDto> GetVehicleInfoQuery(string licensePlate);
}