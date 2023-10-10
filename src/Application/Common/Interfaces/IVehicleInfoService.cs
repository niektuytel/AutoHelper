using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleRelatedServices;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleInfoService
{
    Task<bool> ValidVehicle(string licensePlate);
    Task<VehicleBriefInfoItemDto?> GetVehicleBriefInfo(string licensePlate);
    Task<VehicleInfoItemDto> GetVehicleInfoQuery(string licensePlate);
    Task<GarageServiceType[]> GetRelatedServiceTypesByLicencePlate(string licensePlate);
}