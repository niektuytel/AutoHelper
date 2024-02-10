using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Infrastructure.Services;

internal class VehicleTimelineService : IVehicleTimelineService
{
    public async Task<List<VehicleTimelineItem>> InsertableTimelineItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> defectsBatch, IEnumerable<VehicleInspectionNotificationDtoItem> inspectionsBatch, List<VehicleServiceLogItem> serviceLogsBatch, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
    {
        var vehicleTimelinesToInsert = new List<VehicleTimelineItem>();

        // handle failed MOTs
        var defects = defectsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
        var failedMOTsToInsert = await NewFailedMOTItems(vehicle, defects, defectDescriptions);
        if (failedMOTsToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(failedMOTsToInsert);
        }

        // handle succeeded MOTs
        var inspections = inspectionsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
        var succeededMOTsToInsert = await NewSucceededMOTItems(vehicle, inspections);
        if (succeededMOTsToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(succeededMOTsToInsert);
        }

        // handle owner changes
        var ownerChangedToInsert = await NewOwnerChangedItem(vehicle);
        if (ownerChangedToInsert != null)
        {
            vehicleTimelinesToInsert.Add(ownerChangedToInsert);
        }

        // handle servicelogs changes
        var serviceLogs = serviceLogsBatch!.Where(x =>
            x.VehicleLicensePlate == vehicle.LicensePlate &&
            x.Status != Domain.VehicleServiceLogStatus.NotVerified
        );

        var serviceLogsChangedToInsert = await NewServiceLogItems(vehicle, serviceLogs);
        if (serviceLogsChangedToInsert?.Any() == true)
        {
            vehicleTimelinesToInsert.AddRange(serviceLogsChangedToInsert);
        }

        return vehicleTimelinesToInsert;
    }

    private async Task<List<VehicleTimelineItem>> NewFailedMOTItems(VehicleLookupItem vehicle, IEnumerable<VehicleDetectedDefectDtoItem> detectedDefects, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();

        // No defects found
        if (detectedDefects?.Any() != true)
        {
            return itemsToInsert;
        }

        var groupedByDate = detectedDefects.GroupBy(x => x.DetectionDate);
        foreach (var group in groupedByDate)
        {
            if (vehicle.Timeline?.Any(x => x.Date == group.Key) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateFailedMOTItem(vehicle.LicensePlate, group, defectDescriptions);
            itemsToInsert.Add(item);
        }

        return itemsToInsert;
    }

    private static VehicleTimelineItem CreateFailedMOTItem(string licensePlate, IGrouping<DateTime, VehicleDetectedDefectDtoItem> group, IEnumerable<VehicleDetectedDefectDescriptionDtoItem> defectDescriptions)
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

            extraData.Add(new(description, information.DefectArticleNumber));
        }

        // set the property back to serialize and store the updates
        timelineItem.ExtraData = extraData;

        var total = group.Select(x => x.DetectedAmount).Sum();
        timelineItem.Description = $"Er waren {total} opmerkingen";

        return timelineItem;
    }

    private async Task<List<VehicleTimelineItem>> NewSucceededMOTItems(VehicleLookupItem vehicle, IEnumerable<VehicleInspectionNotificationDtoItem> notifications)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();

        // No notifications found
        if (notifications?.Any() != true)
        {
            return itemsToInsert;
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

            var item = CreateSucceededMOTItem(vehicle.LicensePlate, notification);
            itemsToInsert.Add(item);
        }

        return itemsToInsert;
    }

    private static VehicleTimelineItem CreateSucceededMOTItem(string licensePlate, VehicleInspectionNotificationDtoItem notification)
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

    private async Task<VehicleTimelineItem?> NewOwnerChangedItem(VehicleLookupItem vehicle)
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

        var item = CreateOwnerChangeItem(vehicle.LicensePlate, (DateTime)vehicle.DateOfAscription);
        return item;
    }

    private static VehicleTimelineItem CreateOwnerChangeItem(string licensePlate, DateTime dateOfAscription)
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

    private async Task<List<VehicleTimelineItem>> NewServiceLogItems(VehicleLookupItem vehicle, IEnumerable<VehicleServiceLogItem> serviceLogs)
    {
        var itemsToInsert = new List<VehicleTimelineItem>();

        // No serviceLogs found
        if (serviceLogs?.Any() != true)
        {
            return itemsToInsert;
        }

        var items = new List<VehicleTimelineItem>();
        foreach (var serviceLog in serviceLogs)
        {
            if (vehicle.Timeline?.Any(x => x.Date.Date == serviceLog!.Date.Date) == true)
            {
                // Already exists
                continue;
            }

            var item = CreateServiceLogItem(serviceLog);
            itemsToInsert.Add(item);
        }

        return itemsToInsert;
    }

    public VehicleTimelineItem CreateServiceLogItem(VehicleServiceLogItem serviceLog)
    {
        var extraData = new List<Tuple<string, string>>();
        var type = VehicleTimelineType.Service;
        var title = "Onderhoud";

        if (serviceLog.Type == GarageServiceType.Repair)
        {
            type = VehicleTimelineType.Repair;
            title = "Reparatie";
        }
        else if (serviceLog.Type == GarageServiceType.Inspection)
        {
            type = VehicleTimelineType.Inspection;
            title = "Controle";
        }

        if (!string.IsNullOrEmpty(serviceLog.Title))
        {
            extraData.Add(new($"Soort {title.ToLower()}", serviceLog.Title));
        }

        if (!string.IsNullOrEmpty(serviceLog.Notes))
        {
            extraData.Add(new("Technische aantekening", serviceLog.Notes));
        }

        if (serviceLog.ExpectedNextDate != null && serviceLog.ExpectedNextDate != DateTime.MinValue)
        {
            extraData.Add(new("Volgende onderhoudsbeurt op", ((DateTime)serviceLog.ExpectedNextDate).ToShortDateString()));
        }

        if (serviceLog.ExpectedNextOdometerReading != null && serviceLog.ExpectedNextOdometerReading != 0)
        {
            extraData.Add(new("Volgende onderhoudsbeurt op", $"{serviceLog.ExpectedNextOdometerReading} km"));
        }

        if (!string.IsNullOrEmpty(serviceLog.AttachedFile))
        {
            extraData.Add(new("Bijlage", $"{nameof(serviceLog.AttachedFile)}:{serviceLog.AttachedFile}"));
        }

        var timelineItem = new VehicleTimelineItem()
        {
            Id = Guid.NewGuid(),
            VehicleLicensePlate = serviceLog.VehicleLicensePlate,
            VehicleServiceLogId = serviceLog.Id,
            Date = serviceLog.Date.Date,
            Title = title,
            Description = serviceLog.Description ?? "",
            Type = type,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = extraData
        };

        return timelineItem;
    }

}
