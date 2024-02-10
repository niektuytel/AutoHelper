using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleService
{
    Task<VehicleSpecificationsCardItem?> GetVehicleByLicensePlateAsync(string licensePlate);
    Task<VehicleSpecificationsDtoItem> GetSpecificationsByLicensePlateAsync(string licensePlate);
    Task<VehicleTechnicalDtoItem?> GetTechnicalBriefByLicensePlateAsync(string licensePlate);
    Task<IEnumerable<VehicleDetectedDefectDescriptionDtoItem>> GetDetectedDefectDescriptionsAsync();
    Task<IEnumerable<VehicleDetectedDefectDtoItem>> GetVehicleDetectedDefects(List<string> licensePlates);
    Task<IEnumerable<VehicleInspectionNotificationDtoItem>> GetVehicleInspectionNotifications(List<string> licensePlates);
    Task<VehicleBasicsDtoItem> GetBasicVehicle(string licensePlate);
    Task<IEnumerable<VehicleBasicsDtoItem>> GetVehicleBasicsWithMOTRequirement(int offset, int limit);
    Task<int> GetVehicleBasicsWithMOTRequirementCount();
    bool UpdateVehicleRecord(VehicleBasicsDtoItem? vehicle, VehicleLookupItem vehicleLookup, DateTime? upsertOnlyLastModifiedOlderThan = null);
    VehicleLookupItem CreateVehicleRecord(VehicleBasicsDtoItem vehicle);
}
