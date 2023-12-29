using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Application.Common.Interfaces;

public interface IVehicleTimelineService
{
    Task<List<VehicleTimelineItem>> InsertableTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> defectsBatch, IEnumerable<VehicleInspectionNotificationDtoItem> inspectionsBatch, List<VehicleServiceLogItem> serviceLogsBatch, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions);
    VehicleTimelineItem CreateServiceLogItem(VehicleServiceLogItem serviceLog);
}