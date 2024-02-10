using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Extentions;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class VehicleService : IVehicleService
{
    private readonly IRDWApiClient _rdwService;

    public VehicleService(IRDWApiClient rdwService)
    {
        _rdwService = rdwService;
    }

    public async Task<VehicleSpecificationsCardItem?> GetVehicleByLicensePlateAsync(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        if (data?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        var from = data.GetSafeDateYearValue("datum_eerste_toelating_dt");
        var fromText = from != 0 ? $" uit {from}" : string.Empty;

        var mark = data.GetSafeValue("merk").ToUpper();
        var tradingMark = data.GetSafeValue("handelsbenaming").ToUpper()
            .Replace($"{mark} ", "");

        //.ToCamelCase()
        var brandText = $"{mark.ToTitleCase()} ({tradingMark.ToTitleCase()}){fromText}";
        var mileage = data.GetSafeValue("tellerstandoordeel");
        var response = new VehicleSpecificationsCardItem
        {
            LicensePlate = licensePlate,
            Type = GetVehicleType(data),
            Brand = brandText,
            Mileage = mileage,
        };

        if (DateTime.TryParse(data.GetSafeDateValue("vervaldatum_apk_dt"), out var motExpiry))
        {
            response.DateOfMOTExpiry = motExpiry;
        }

        if (DateTime.TryParse(data.GetSafeDateValue("datum_tenaamstelling_dt"), out var dateOfAscription))
        {
            response.DateOfAscription = dateOfAscription;
        }

        var fuelInfo = await _rdwService.GetVehicleFuel(licensePlate);
        if (fuelInfo?.HasValues == true)
        {
            var amount = fuelInfo.GetSafeDecimalValue("brandstofverbruik_gecombineerd");
            var consumptionText = amount != 0
                ? $"{(100 / (amount / 100M)):F0}km op 1 liter {fuelInfo.GetSafeValue("brandstof_omschrijving").ToLower()}"
                : "Niet geregistreerd";

            response.Consumption = consumptionText;
        }

        return response;
    }

    public async Task<VehicleSpecificationsDtoItem> GetSpecificationsByLicensePlateAsync(string licensePlate)
    {
        var response = new VehicleSpecificationsDtoItem();
        var data = await _rdwService.GetVehicle(licensePlate);

        if (!data.HasValues)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        response.Data.Add(_rdwService.GetGeneralData(data));
        response.Data.Add(_rdwService.GetExpirationDatesAndHistory(data));
        response.Data.Add(_rdwService.GetWeightsData(data));
        response.Data.Add(_rdwService.GetCounterReadings(data));
        response.Data.Add(_rdwService.GetVehicleStatus(data));
        response.Data.Add(_rdwService.GetRecallData(data));
        response.Data.Add(_rdwService.GetMotorData(data));

        var fuelInfo = await _rdwService.GetVehicleFuel(licensePlate);
        if (fuelInfo.HasValues)
        {
            response.Data.Add(_rdwService.GetEnvironmentalPerformance(fuelInfo));
            response.Data.Add(_rdwService.GetEmissions(fuelInfo));
        }

        response.Data.Add(_rdwService.GetCharacteristicsData(data));

        var shafts = await _rdwService.GetVehicleShafts(licensePlate);
        response.Data.Add(_rdwService.GetShaftsData(shafts));
        response.Data.Add(_rdwService.GetFiscalData(data));
        return response;
    }

    public async Task<VehicleType> GetVehicleType(string licensePlate)
    {
        var data = await _rdwService.GetVehicle(licensePlate);
        if (data?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        return GetVehicleType(data);
    }

    // TODO: Need better investigation
    // can add more cases based on other vehicle kinds present in the RDW data.
    private static VehicleType GetVehicleType(JToken data)
    {
        // Check "voertuigsoort" field for various types
        var vehicleKind = data?["voertuigsoort"]?.ToString();

        // Check weight for HeavyCar
        if (int.TryParse(data?["technische_max_massa_voertuig"]?.ToString(), out int weight) && weight > 3500)
        {
            return VehicleType.HeavyCar;
        }

        switch (vehicleKind)
        {
            case "Personenauto":
                return VehicleType.LightCar;
            case "Driewielig motorrijtuig":
                return VehicleType.Motorcycle;
            case "Land- of bosbouwtrekker":
                return VehicleType.Tractor;
            case "Land- of bosb aanhw of getr uitr stuk":
                return VehicleType.Tractor;

            default:
                break;
        }

        // Check Taxi
        if (data?["taxi_indicator"]?.ToString() == "Ja")
        {
            return VehicleType.Taxi;
        }


        // If no matches, return Other
        return VehicleType.Any;
    }

    public async Task<VehicleTechnicalDtoItem?> GetTechnicalBriefByLicensePlateAsync(string licensePlate)
    {
        var vehicleData = await _rdwService.GetVehicle(licensePlate);
        if (vehicleData?.HasValues != true)
        {
            throw new NotFoundException("Vehicle data not found.");
        }

        var vehicleBrief = MapToVehicleTechnicalBriefDtoItem(licensePlate, vehicleData);
        await PopulateFuelInformation(vehicleBrief);

        return vehicleBrief;
    }

    private VehicleTechnicalDtoItem MapToVehicleTechnicalBriefDtoItem(string licensePlate, JToken vehicleData)
    {
        return new VehicleTechnicalDtoItem
        {
            LicensePlate = licensePlate,
            Brand = vehicleData.GetSafeValue("merk"),
            Model = vehicleData.GetSafeValue("handelsbenaming"),
            YearOfFirstAdmission = vehicleData.GetSafeDateYearValue("datum_eerste_toelating_dt"),
            MOTExpiryDate = vehicleData.GetSafeDateValue("vervaldatum_apk_dt"),
            Mileage = vehicleData.GetSafeValue("tellerstandoordeel")
        };
    }

    private async Task PopulateFuelInformation(VehicleTechnicalDtoItem vehicleBrief)
    {
        var fuelInfo = await _rdwService.GetVehicleFuel(vehicleBrief.LicensePlate);
        if (fuelInfo?.HasValues == true)
        {
            var combinedFuelConsumption = fuelInfo.GetSafeDecimalValue("brandstofverbruik_gecombineerd");
            vehicleBrief.FuelType = fuelInfo.GetSafeValue("brandstof_omschrijving");
            vehicleBrief.CombinedFuelConsumption = combinedFuelConsumption;

            if (combinedFuelConsumption != 0)
            {
                vehicleBrief.FuelEfficiency = 100M / (combinedFuelConsumption * 100M);
            }
        }
    }

    public async Task<IEnumerable<VehicleDetectedDefectDescriptionDtoItem>> GetDetectedDefectDescriptionsAsync()
    {
        return await _rdwService.GetDetectedDefectDescriptions();
    }

    public async Task<VehicleBasicsDtoItem> GetBasicVehicle(string licensePlate)
    {
        return await _rdwService.GetBasicVehicle(licensePlate);
    }

    public async Task<IEnumerable<VehicleBasicsDtoItem>> GetVehicleBasicsWithMOTRequirement(int offset, int limit)
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirement(offset, limit);
    }

    public async Task<int> GetVehicleBasicsWithMOTRequirementCount()
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirementCount();
    }


    public async Task<IEnumerable<VehicleDetectedDefectDtoItem>> GetVehicleDetectedDefects(List<string> licensePlates)
    {
        var limit = 2000;
        var offset = 0;

        var defects = new List<VehicleDetectedDefectDtoItem>();
        do
        {
            var items = await _rdwService.GetVehicleDetectedDefects(licensePlates, offset, limit);
            defects.AddRange(items);
            offset++;
        }
        while (defects.Count() == (limit * offset));
        return defects.ToArray();
    }

    public async Task<IEnumerable<VehicleInspectionNotificationDtoItem>> GetVehicleInspectionNotifications(List<string> licensePlates)
    {
        var limit = 2000;
        var offset = 0;

        var inspections = new List<VehicleInspectionNotificationDtoItem>();
        do
        {
            var items = await _rdwService.GetVehicleInspectionNotifications(licensePlates, offset, limit);
            inspections.AddRange(items);
            offset++;
        }
        while (inspections.Count() == (limit * offset));
        return inspections.ToArray();
    }

    public bool UpdateVehicleRecord(VehicleBasicsDtoItem? vehicle, VehicleLookupItem vehicleLookup, DateTime? upsertOnlyLastModifiedOlderThan = null)
    {
        bool somethingChanged = VehicleRecordHasChanges(vehicleLookup, vehicle, upsertOnlyLastModifiedOlderThan);
        if (!somethingChanged)
        {
            return false;
        }

        // Update vehicleLookup details
        vehicleLookup.DateOfMOTExpiry = vehicle.MOTExpiryDateDt;
        vehicleLookup.DateOfAscription = vehicle.RegistrationDateDt;
        vehicleLookup.LastModified = DateTime.UtcNow;
        vehicleLookup.LastModifiedBy = $"system";

        return true;
    }

    private bool VehicleRecordHasChanges(VehicleLookupItem vehicleLookup, VehicleBasicsDtoItem vehicle, DateTime? upsertOnlyLastModifiedOlderThan = null)
    {
        var sameExpirationDate = vehicleLookup.DateOfMOTExpiry == vehicle.MOTExpiryDateDt;
        var sameRegistrationDate = vehicleLookup.DateOfAscription == vehicle.RegistrationDateDt;

        if (upsertOnlyLastModifiedOlderThan != null && vehicleLookup!.LastModified >= upsertOnlyLastModifiedOlderThan)
        {
            return false;
        }
        else if (sameExpirationDate && sameRegistrationDate)
        {
            return false;
        }

        return true;
    }

    public VehicleLookupItem CreateVehicleRecord(VehicleBasicsDtoItem vehicle)
    {
        var vehicleLookup = new VehicleLookupItem
        {
            LicensePlate = vehicle.LicensePlate,
            DateOfMOTExpiry = vehicle.MOTExpiryDateDt,
            DateOfAscription = vehicle.RegistrationDateDt,
            Created = DateTime.UtcNow,
            CreatedBy = $"system",
            LastModified = DateTime.UtcNow,
            LastModifiedBy = $"system"
        };

        return vehicleLookup;
    }


}
