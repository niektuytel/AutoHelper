using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleDefects;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<string>> GetAllLicensePlatesAsync();
    Task<VehicleBriefDtoItem?> GetVehicleByLicensePlateAsync(string licensePlate);
    Task<VehicleSpecificationsDto> GetSpecificationsByLicensePlateAsync(string licensePlate);
    Task<VehicleType> GetVehicleTypeByLicensePlateAsync(string licensePlate);
    Task<bool> IsVehicleValidAsync(string licensePlate);
    Task<VehicleTechnicalBriefDtoItem?> GetTechnicalBriefByLicensePlateAsync(string licensePlate);
    Task<RDWDetectedDefect[]> GetDefectHistoryByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<RDWDetectedDefectDescription>> GetDetectedDefectDescriptionsAsync();
    Task ForEachDetectedDefectAsync(Func<IEnumerable<RDWDetectedDefect>, Task> handleVehicle);
    Task ForEachInspectionNotificationAsync(Func<IEnumerable<RDWInspectionNotification>, Task> handleVehicle);
}
