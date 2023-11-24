using System.Collections.Generic;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimeline;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Common.Models;
using Azure;
using Azure.Core;
using Force.DeepCloner;
using GoogleApi.Entities.Maps.Directions.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoHelper.Infrastructure.Services;

internal class VehicleService : IVehicleService
{
    private readonly RDWApiClient _rdwService;

    public VehicleService(RDWApiClient rdwService)
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
        var brandText = $"{mark.ToPascalCase()} ({tradingMark.ToPascalCase()}){fromText}";
        var mileage = data.GetSafeValue("tellerstandoordeel");
        var response = new VehicleSpecificationsCardItem
        {
            LicensePlate = licensePlate,
            Type = GetVehicleType(data),
            Brand = brandText,
            Mileage = mileage,
        };

        if(DateTime.TryParse(data.GetSafeDateValue("vervaldatum_apk_dt"), out var motExpiry))
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
                ? $"{(100 / (amount/100M)):F0}km op 1 liter {fuelInfo.GetSafeValue("brandstof_omschrijving").ToLower()}"
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

    public async Task<VehicleLookupType> GetVehicleType(string licensePlate)
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
    private static VehicleLookupType GetVehicleType(JToken data)
    {
        // Check "voertuigsoort" field for various types
        var vehicleKind = data?["voertuigsoort"]?.ToString();

        // Check weight for HeavyCar
        if (int.TryParse(data?["technische_max_massa_voertuig"]?.ToString(), out int weight) && weight > 3500)
        {
            return VehicleLookupType.HeavyCar;
        }

        switch (vehicleKind)
        {
            case "Personenauto":
                return VehicleLookupType.LightCar;
            case "Driewielig motorrijtuig":
                return VehicleLookupType.Motorcycle;
            case "Land- of bosbouwtrekker":
                return VehicleLookupType.Tractor;
            case "Land- of bosb aanhw of getr uitr stuk":
                return VehicleLookupType.Tractor;

            default:
                break;
        }

        // Check Taxi
        if (data?["taxi_indicator"]?.ToString() == "Ja")
        {
            return VehicleLookupType.Taxi;
        }


        // If no matches, return Other
        return VehicleLookupType.Other;
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

    public async Task<IEnumerable<VehicleBasicsDtoItem>> GetVehicleBasicsWithMOTRequirement(int offset, int limit)
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirement(offset, limit);
    }

    public async Task<int> GetVehicleBasicsWithMOTRequirementCount()
    {
        return await _rdwService.GetVehicleBasicsWithMOTRequirementCount();
    }

    private async Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> FailedMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> detectedDefects, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();
        var itemsToUpdate = new List<VehicleTimelineItem>();

        // No defects found
        if (detectedDefects?.Any() != true)
        {
            return (itemsToInsert, itemsToUpdate);
        }

        var groupedByDate = detectedDefects.GroupBy(x => x.DetectionDate);
        foreach (var group in groupedByDate)
        {
            if (vehicle.Timeline?.Any(x => x.Date == group.Key) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateFailedMOTTimelineItem(vehicle.LicensePlate, group, defectDescriptions);
            itemsToInsert.Add(item);
        }

        return (itemsToInsert, itemsToUpdate);
    }

    private async Task<(List<VehicleTimelineItem> failedMOTsToInsert, List<VehicleTimelineItem> failedMOTsToUpdate)> SucceededMOTTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleInspectionNotificationDtoItem> notifications)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();
        var itemsToUpdate = new List<VehicleTimelineItem>();

        // No notifications found
        if (notifications?.Any() != true)
        {
            return (itemsToInsert, itemsToUpdate);
        }

        var items = new List<VehicleTimelineItem>();
        var groupedByDate = notifications.GroupBy(x => x.DateTimeByAuthority);
        foreach (var notification in notifications)
        {
            if (vehicle.Timeline?.Any(x => x.Date == notification!.DateTimeByAuthority) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateSucceededMOTTimelineItem(vehicle.LicensePlate, notification);
            itemsToInsert.Add(item);
        }

        return (itemsToInsert, itemsToUpdate);
    }

    private async Task<VehicleTimelineItem?> OwnerChangedTimelineItem(VehicleLookupItem vehicle)
    {
        var entity = vehicle.Timeline?.FirstOrDefault(x => 
            x.Type == VehicleTimelineType.OwnerChange &&
            x.Date == vehicle.DateOfAscription
        );

        // Already exists or has invalid date
        if (entity != null || vehicle.DateOfAscription == null || vehicle.DateOfAscription == DateTime.MinValue)
        {
            return null;
        }

        var item = CreateOwnerChangeTimelineItem(vehicle.LicensePlate, (DateTime)vehicle.DateOfAscription);
        return item;
    }

    private async Task<(List<VehicleTimelineItem> serviceLogsChangedToInsert, List<VehicleTimelineItem> serviceLogsChangedToUpdate)> ServiceLogsTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleServiceLogItem> serviceLogs)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();
        var itemsToUpdate = new List<VehicleTimelineItem>();

        // No serviceLogs found
        if (serviceLogs?.Any() != true)
        {
            return (itemsToInsert, itemsToUpdate);
        }

        var items = new List<VehicleTimelineItem>();
        foreach (var serviceLog in serviceLogs)
        {
            if (vehicle.Timeline?.Any(x => x.Date.Date == serviceLog!.Date.Date) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateServiceLogTimelineItem(vehicle.LicensePlate, serviceLog);
            itemsToInsert.Add(item);
        }

        return (itemsToInsert, itemsToUpdate);
    }

    private static VehicleTimelineItem CreateFailedMOTTimelineItem(string licensePlate, IGrouping<DateTime, VehicleDetectedDefectDtoItem> group, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = group.Key,
            Title = "APK afgekeurd",
            Type = VehicleTimelineType.FailedMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new List<Tuple<string, string>>()
        };

        // avoid repeated deserialization
        var extraData = timelineItem.ExtraData;

        foreach (var defect in group)
        {
            var information = defectDescriptions.First(x => x.Identification == defect.Identifier);
            var description = information.Description;
            if (defect.DetectedAmount > 1)
            {
                description += $" ({defect.DetectedAmount}x)";
            }

            extraData.Add(new (description, information.DefectArticleNumber));
        }

        // set the property back to serialize and store the updates
        timelineItem.ExtraData = extraData;

        var total = group.Select(x => x.DetectedAmount).Sum();
        timelineItem.Description = $"Er waren {total} opmerkingen";

        return timelineItem;
    }

    private static VehicleTimelineItem CreateSucceededMOTTimelineItem(string licensePlate, VehicleInspectionNotificationDtoItem notification)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = notification.DateTimeByAuthority,
            Title = "APK goedgekeurd",
            Description = "",
            Type = VehicleTimelineType.SucceededMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new List<Tuple<string, string>>()
            {
                new ("Verval datum", notification.ExpiryDateTime.ToShortDateString())
            }
        };

        return timelineItem;
    }

    private static VehicleTimelineItem CreateOwnerChangeTimelineItem(string licensePlate, DateTime dateOfAscription)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = dateOfAscription,
            Title = "Nieuwe eigenaar",
            Description = "",
            Type = VehicleTimelineType.OwnerChange,
            Priority = VehicleTimelinePriority.Low,
            ExtraData = new List<Tuple<string, string>>()
        };

        return timelineItem;
    }

    private static VehicleTimelineItem CreateServiceLogTimelineItem(string licensePlate, VehicleServiceLogItem serviceLog)
    {
        var type = VehicleTimelineType.Service;
        var title = "Onderhoud";
        if (serviceLog.Type == GarageServiceType.Repair)
        {
            type = VehicleTimelineType.Repair;
            title = "Reparatie";
        }

        var extraData = new List<Tuple<string, string>>();
        if (string.IsNullOrEmpty(serviceLog.Notes))
        {
            extraData.Add(new ("Notities", serviceLog.Notes));
        }
        else if (serviceLog.ExpectedNextDate != null && serviceLog.ExpectedNextDate != DateTime.MinValue)
        {
            extraData.Add(new("Volgende onderhoudsbeurt bij datum", ((DateTime)serviceLog.ExpectedNextDate).ToShortDateString()));
        }
        else if (serviceLog.ExpectedNextOdometerReading != null && serviceLog.ExpectedNextOdometerReading != 0)
        {
            extraData.Add(new("Volgende onderhoudsbeurt bij km", $"{serviceLog.ExpectedNextOdometerReading} km"));
        }

        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = licensePlate,
            Date = serviceLog.Date.Date,
            Title = title,
            Description = serviceLog.Description ?? "",
            Type = type,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = extraData
        };

        return timelineItem;
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

    public async Task<(List<VehicleTimelineItem> itemsToInsert, List<VehicleTimelineItem> itemsToUpdate)> UpsertTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> defectsBatch, IEnumerable<VehicleInspectionNotificationDtoItem> inspectionsBatch, List<VehicleServiceLogItem> serviceLogsBatch, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
    {
        var vehicleTimelinesToInsert = new List<VehicleTimelineItem>();
        var vehicleTimelinesToUpdate = new List<VehicleTimelineItem>();

        // handle failed MOTs
        var defects = defectsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
        var (failedMOTsToInsert, _) = await FailedMOTTimelineItems(vehicle, defects, defectDescriptions);
        if (failedMOTsToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(failedMOTsToInsert);
        }

        // handle succeeded MOTs
        var inspections = inspectionsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
        var (succeededMOTsToInsert, _) = await SucceededMOTTimelineItems(vehicle, inspections);
        if (succeededMOTsToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(succeededMOTsToInsert);
        }

        // handle owner changes
        var ownerChangedToInsert = await OwnerChangedTimelineItem(vehicle);
        if (ownerChangedToInsert != null)
        {
            vehicleTimelinesToInsert.Add(ownerChangedToInsert);
        }

        // handle servicelogs changes
        var serviceLogs = serviceLogsBatch!.Where(x => x.VehicleLicensePlate == vehicle.LicensePlate);
        var (serviceLogsChangedToInsert, _) = await ServiceLogsTimelineItems(vehicle, serviceLogs);
        if (serviceLogsChangedToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(serviceLogsChangedToInsert);
        }

        return (vehicleTimelinesToInsert, vehicleTimelinesToUpdate);
    }

}
