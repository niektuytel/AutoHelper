using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;
public interface IRDWApiClient
{
    Task<IEnumerable<RDWCompany>> GetAllCompanies();
    Task<IEnumerable<RDWCompany>> GetAllCompanies(int offset, int limit, bool includeFiltering = true);
    Task<int> GetAllCompaniesCount(bool includeFiltering = true);
    Task<IEnumerable<RDWCompanyService>> GetAllServices(bool skipOnUnkownServiceType = true);
    Task<VehicleBasicsDtoItem> GetBasicVehicle(string licensePlate);
    VehicleInfoSectionItem GetCharacteristicsData(JToken data);
    VehicleInfoSectionItem GetCounterReadings(JToken data);
    Task<IEnumerable<VehicleDetectedDefectDescriptionDtoItem>> GetDetectedDefectDescriptions();
    VehicleInfoSectionItem GetEmissions(JToken fuelInfo);
    VehicleInfoSectionItem GetEnvironmentalPerformance(JToken fuelInfo);
    VehicleInfoSectionItem GetExpirationDatesAndHistory(JToken data);
    VehicleInfoSectionItem GetFiscalData(JToken data);
    VehicleInfoSectionItem GetGeneralData(JToken data);
    VehicleInfoSectionItem GetMotorData(JToken data);
    VehicleInfoSectionItem GetRecallData(JToken data);
    VehicleInfoSectionItem GetShaftsData(JArray? shafts);
    Task<JToken?> GetVehicle(string licensePlate);
    Task<IEnumerable<VehicleBasicsDtoItem>> GetVehicleBasicsWithMOTRequirement(int offset, int limit);
    Task<int> GetVehicleBasicsWithMOTRequirementCount();
    string GetVehicleCounterReadingsDescription(string judgement);
    Task<IEnumerable<VehicleDetectedDefectDtoItem>> GetVehicleDetectedDefects(List<string> licensePlates, int offset, int limit);
    Task<JToken?> GetVehicleFuel(string licensePlate);
    Task<IEnumerable<VehicleInspectionNotificationDtoItem>> GetVehicleInspectionNotifications(List<string> licensePlates, int offset, int limit);
    Task<JArray?> GetVehicleShafts(string licensePlate);
    VehicleInfoSectionItem GetVehicleStatus(JToken data);
    VehicleInfoSectionItem GetWeightsData(JToken data);
}