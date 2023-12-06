using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using Newtonsoft.Json.Linq;

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
    Task<(List<VehicleTimelineItem> itemsToInsert, List<VehicleTimelineItem> itemsToUpdate)> UpsertTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> defectsBatch, IEnumerable<VehicleInspectionNotificationDtoItem> inspectionsBatch, List<VehicleServiceLogItem> serviceLogsBatch, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions);
    VehicleTimelineItem CreateServiceLogTimelineItem(string licensePlate, VehicleServiceLogItem serviceLog);
}
