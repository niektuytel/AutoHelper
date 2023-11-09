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
    Task ForEachVehicleBasicsInBatches(Func<IEnumerable<RDWVehicleBasics>, Task> onVehicleBatch);
    Task<IEnumerable<RDWDetectedDefectDescription>> GetDetectedDefectDescriptionsAsync();
    Task<List<VehicleTimelineItem>> GetVehicleUpdatedTimeline(
        List<VehicleTimelineItem> timeline, 
        RDWVehicleBasics vehicle, 
        IEnumerable<RDWDetectedDefectDescription> defectDescriptions
    );
    bool MOTIsRequired(string europeanVehicleCategory);
}
