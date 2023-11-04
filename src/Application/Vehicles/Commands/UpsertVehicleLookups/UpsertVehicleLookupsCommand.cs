using System.Data.Entity;
using System.Threading;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;

public record UpsertVehicleLookupsCommand : IRequest
{
    public UpsertVehicleLookupsCommand()
    {
        MaxInsertAmount = 0;
        MaxUpdateAmount = 0;
        UpsertTimeline = true;
        UpsertServiceLogs = true;
    }

    public UpsertVehicleLookupsCommand(int maxInsertAmount)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = 0;
        UpsertTimeline = true;
        UpsertServiceLogs = true;
    }

    public UpsertVehicleLookupsCommand(int maxInsertAmount, int maxUpdateAmount, bool updateTimeline, bool updateServiceLogs)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        UpsertTimeline = updateTimeline;
        UpsertServiceLogs = updateServiceLogs;
    }

    public int MaxInsertAmount { get; set; }
    public int MaxUpdateAmount { get; set; }
    public bool UpsertTimeline { get; set; }
    public bool UpsertServiceLogs { get; set; }
}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<UpsertVehicleLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
    }

    public async Task<Unit> Handle(UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {

        // TODO: Implement the max insert and update amount functionality
        var startMaxInsertAmount = request.MaxInsertAmount;
        var startMaxUpdateAmount = request.MaxUpdateAmount;

        // TODO: Logging of the amount of inserts and updates to the hangfire dashboard
        if (request.UpsertTimeline)
        {
            var detectedDefectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();

            await _vehicleService.ForEachDetectedDefectAsync(async bulkByLicensePlate =>
            {
                var licensePlate = bulkByLicensePlate.First().LicensePlate;
                var timeline = CreateVehicleFailedMOTTimeline(bulkByLicensePlate, detectedDefectDescriptions);

                await UpsertVehicleLookup(licensePlate, timeline, cancellationToken);
            });

            await _vehicleService.ForEachInspectionNotificationAsync(async bulkByLicensePlate =>
            {
                var licensePlate = bulkByLicensePlate.First().LicensePlate;
                var timeline = CreateVehicleSucceededMOTTimeline(licensePlate, bulkByLicensePlate);

                await UpsertVehicleLookup(licensePlate, timeline, cancellationToken);
            });
        }

        if (request.UpsertServiceLogs)
        {
            // TODO: implement
            throw new NotImplementedException();
        }

  
        return Unit.Value;
    }

    private List<VehicleTimelineItem> CreateVehicleFailedMOTTimeline(IEnumerable<RDWDetectedDefect> bulkByLicensePlate, IEnumerable<RDWDetectedDefectDescription> defectDescriptions)
    {
        var timeline = new List<VehicleTimelineItem>();
        var groupedByDate = bulkByLicensePlate.GroupBy(x => x.DetectionDate);
        foreach (var group in groupedByDate)
        {
            var timelineItem = CreateFailedMOTTimelineItem(group, defectDescriptions);
            timeline.Add(timelineItem);
        }

        return timeline.ToList();
    }

    private List<VehicleTimelineItem> CreateVehicleSucceededMOTTimeline(string licensePlate, IEnumerable<RDWInspectionNotification> bulkByLicensePlate)
    {
        var lookup = _dbContext.VehicleLookups
            .Include(x => x.Timeline)
            .FirstOrDefault(x => x.LicensePlate == licensePlate);

        var timeline = new List<VehicleTimelineItem>();
        var groupedByDate = bulkByLicensePlate.GroupBy(x => x.DateTimeByAuthority);
        foreach (var group in groupedByDate)
        {
            var item = group.First();
            var alreadyExist = lookup.Timeline.Any(x => x.Date == item.DateTimeByAuthority);
            if (alreadyExist)
            {
                continue;
            }

            var timelineItem = CreateSucceededMOTTimelineItem(item);
            timeline.Add(timelineItem);
        }

        return timeline.ToList();
    }

    private VehicleTimelineItem CreateFailedMOTTimelineItem(IGrouping<DateTime, RDWDetectedDefect> group, IEnumerable<RDWDetectedDefectDescription> defectDescriptions)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Date = group.Key,
            Title = "APK afgekeurd",
            Type = VehicleTimelineType.FailedMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new Dictionary<string, string>()
        };

        foreach (var defect in group)
        {
            var information = defectDescriptions.First(x => x.Identification == defect.Identifier);
            var description = information.Description;
            if (defect.DetectedAmount > 1)
            {
                description += $" ({defect.DetectedAmount}x)";
            }

            timelineItem.ExtraData.Add(information.DefectArticleNumber, description);
        }

        return timelineItem;
    }

    private VehicleTimelineItem CreateSucceededMOTTimelineItem(RDWInspectionNotification notification)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Date = notification.DateTimeByAuthority,
            Title = "APK goedgekeurd",
            Type = VehicleTimelineType.SucceededMOT,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new Dictionary<string, string>()
        };

        timelineItem.ExtraData.Add("Verval datum", notification.ExpiryDateTime.ToShortDateString());
        return timelineItem;
    }

    private VehicleTimelineItem CreateOwnerChangeTimelineItem(DateTime dateOfAscription)
    {
        var timelineItem = new VehicleTimelineItem()
        {
            Date = dateOfAscription,
            Title = "Nieuwe eigenaar",
            Type = VehicleTimelineType.OwnerChange,
            Priority = VehicleTimelinePriority.Medium,
            ExtraData = new Dictionary<string, string>()
        };

        return timelineItem;
    }

    private async Task UpsertVehicleLookup(string licensePlate, List<VehicleTimelineItem> timeline, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
        if (vehicle == null) return;

        var lookup = await _dbContext.VehicleLookups
            .Include(x => x.Timeline)
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        // Always insert owner change to timeline
        if(!timeline.Any(x => x.Date == vehicle.DateOfAscription))
        {
            var newOwnerLine = CreateOwnerChangeTimelineItem(vehicle.DateOfAscription);
            timeline.Add(newOwnerLine);
        }

        if (lookup == null)
        {
            lookup = new VehicleLookupItem
            {
                LicensePlate = licensePlate,
                MOTExpiryDate = vehicle.MOTExpiryDate,
                DateOfAscription = vehicle.DateOfAscription,
                Timeline = timeline
            };

            _dbContext.VehicleLookups.Add(lookup);
        }
        else
        {
            timeline.AddRange(lookup.Timeline);
            lookup.DateOfAscription = vehicle.DateOfAscription;
            lookup.Timeline = timeline.OrderByDescending(x => x.Date).ToList();

            _dbContext.VehicleLookups.Update(lookup);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}