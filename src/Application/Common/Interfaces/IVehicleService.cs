using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleService
{
    Task<bool> ValidVehicle(string licensePlate);

    Task<VehicleBriefInfoItem?> GetVehicleBriefInfo(string licensePlate);

    //Task<VehicleInformationResponse> GetVehicleInformationAsync(string licensePlate);
}