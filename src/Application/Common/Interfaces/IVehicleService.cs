using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Services;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<string>> GetAllLicensePlatesAsync();
    Task<VehicleBriefDtoItem?> GetVehicleByLicensePlateAsync(string licensePlate);
    Task<VehicleSpecificationsDto> GetSpecificationsByLicensePlateAsync(string licensePlate);
    Task<VehicleType> GetVehicleTypeByLicensePlateAsync(string licensePlate);
    Task<bool> IsVehicleValidAsync(string licensePlate);
    Task<VehicleTechnicalBriefDtoItem?> GetTechnicalBriefByLicensePlateAsync(string licensePlate);
    Task<RDWVehicleDetectedDefect[]> GetDefectHistoryByLicensePlateAsync(string licensePlate);
    Task ForEachVehicleInBatches(Func<IEnumerable<RDWVehicle>, Task> onVehicleBatch);
    Task<IEnumerable<RDWDetectedDefectDescription>> GetDetectedDefectDescriptionsAsync();


    bool MOTIsRequired(string europeanVehicleCategory);
    Task<IEnumerable<RDWVehicleDetectedDefect>> GetVehicleDetectedDefects(List<string> licensePlates);
    Task<IEnumerable<RDWvehicleInspectionNotification>> GetVehicleInspectionNotifications(List<string> licensePlates);
    Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> FailedMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<RDWVehicleDetectedDefect> detectedDefects, IEnumerable<RDWDetectedDefectDescription> defectDescriptions);
    Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> SucceededMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<RDWvehicleInspectionNotification> notifications);
    Task<VehicleTimelineItem?> OwnerChangedTimelineItem(VehicleLookupItem vehicle);
    Task<IEnumerable<RDWVehicleBasics>> GetVehicleBasicsWithMOTRequirement(int offset, int limit);
    Task<int> GetVehicleBasicsWithMOTRequirementCount();
    Task<(List<VehicleTimelineItem> serviceLogsChangedToInsert, List<VehicleTimelineItem> serviceLogsChangedToUpdate)> ServiceLogsTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleServiceLogItem> serviceLogs);
}
