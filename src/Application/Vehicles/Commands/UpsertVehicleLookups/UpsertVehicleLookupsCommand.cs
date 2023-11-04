using System.Data.Entity;
using System.Threading;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
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

    private int _maxInsertAmount = 0;
    private int _maxUpdateAmount = 0;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
    }

    public async Task<Unit> Handle(UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        _maxInsertAmount = request.MaxInsertAmount;
        _maxUpdateAmount = request.MaxUpdateAmount;

        try
        {
            if (request.UpsertTimeline)
            {
                var detectedDefectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();

                await _vehicleService.ForEachDetectedDefectAsync(async bulkByLicensePlate =>
                {
                    var stop = false;
                    var licensePlate = bulkByLicensePlate.First().LicensePlate;
                    var vehicle = await _vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
                    if (vehicle == null) return;

                    var vehicleLookup = await _dbContext.VehicleLookups
                        .Include(x => x.Timeline)
                        .FirstOrDefaultAsync(x => x.LicensePlate == vehicle.LicensePlate, cancellationToken);

                    var timeline = new List<VehicleTimelineItem>();
                    var failedMOTs = UndefinedFailedMOTTimelineItems(vehicleLookup, bulkByLicensePlate, detectedDefectDescriptions);
                    if (failedMOTs?.Any() == true)
                    {
                        timeline.AddRange(failedMOTs);
                    }

                    if (timeline?.Any() == true)
                    {
                        await UpsertVehicleLookup(vehicleLookup, timeline, vehicle, cancellationToken);

                        if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                        {
                            throw new OperationCanceledException($"Given MaxInsertAmount:{_maxInsertAmount} MaxUpdateAmount:{_maxUpdateAmount} reached");
                        }
                    }
                });

                await _vehicleService.ForEachInspectionNotificationAsync(async bulkByLicensePlate =>
                {
                    var licensePlate = bulkByLicensePlate.First().LicensePlate;
                    var vehicle = await _vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
                    if (vehicle == null) return;

                    var vehicleLookup = await _dbContext.VehicleLookups
                        .Include(x => x.Timeline)
                        .FirstOrDefaultAsync(x => x.LicensePlate == vehicle.LicensePlate, cancellationToken);

                    var timeline = new List<VehicleTimelineItem>();
                    var succeededMOTs = UndefinedSucceededMOTTimelineItems(vehicleLookup, bulkByLicensePlate);
                    if (succeededMOTs?.Any() == true)
                    {
                        timeline.AddRange(succeededMOTs);
                    }

                    var ownerChanged = UndefinedOwnerChangedTimelineItem(vehicleLookup, vehicle.DateOfAscription);
                    if (ownerChanged != null)
                    {
                        timeline.Add(ownerChanged);
                    }

                    if (timeline?.Any() == true)
                    {
                        await UpsertVehicleLookup(vehicleLookup, timeline, vehicle, cancellationToken);

                        if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                        {
                            throw new OperationCanceledException($"Given MaxInsertAmount:{_maxInsertAmount} MaxUpdateAmount:{_maxUpdateAmount} reached");
                        }
                    }
                });
            }

            if (request.UpsertServiceLogs)
            {
                // TODO: implement
                throw new NotImplementedException();
            }
}
        catch (OperationCanceledException ex)
        {
            // TODO: Logging of the amount of inserts and updates to the hangfire dashboard
            // TODO: _queueingService.LogInformation();
            return Unit.Value;
        }
  
        return Unit.Value;
    }

    private List<VehicleTimelineItem> UndefinedFailedMOTTimelineItems(VehicleLookupItem vehicleLookup, IEnumerable<RDWDetectedDefect> bulkByLicensePlate, IEnumerable<RDWDetectedDefectDescription> defectDescriptions)
    {
        var timeline = new List<VehicleTimelineItem>();
        var groupedByDate = bulkByLicensePlate.GroupBy(x => x.DetectionDate);
        foreach (var group in groupedByDate)
        {
            if (vehicleLookup.Timeline.Any(x => x.Date == group.Key))
            {
                continue;
            }

            var item = CreateFailedMOTTimelineItem(group, defectDescriptions);
            timeline.Add(item);
        }

        return timeline.ToList();
    }

    private List<VehicleTimelineItem> UndefinedSucceededMOTTimelineItems(VehicleLookupItem vehicleLookup, IEnumerable<RDWInspectionNotification> bulkByLicensePlate)
    {
        var timeline = new List<VehicleTimelineItem>();
        var groupedByDate = bulkByLicensePlate.GroupBy(x => x.DateTimeByAuthority);
        foreach (var group in groupedByDate)
        {
            if (vehicleLookup.Timeline.Any(x => x.Date == group.Key))
            {
                continue;
            }

            var notification = group.First();
            var item = CreateSucceededMOTTimelineItem(notification);
            timeline.Add(item);
        }

        return timeline.ToList();
    }

    private VehicleTimelineItem? UndefinedOwnerChangedTimelineItem(VehicleLookupItem vehicleLookup, DateTime dateOfAscription)
    {
        if (!vehicleLookup.Timeline.Any(x => x.Date == dateOfAscription))
        {
            var item = CreateOwnerChangeTimelineItem(dateOfAscription);
            return item;
        }

        return null;
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
            Priority = VehicleTimelinePriority.Low,
            ExtraData = new Dictionary<string, string>()
        };

        return timelineItem;
    }

    private async Task UpsertVehicleLookup(VehicleLookupItem? vehicleLookup, List<VehicleTimelineItem> timeline, VehicleBriefDtoItem vehicle, CancellationToken cancellationToken)
    {
        
        if (_maxInsertAmount != 0 && vehicleLookup == null)
        {
            vehicleLookup = new VehicleLookupItem
            {
                LicensePlate = vehicle.LicensePlate,
                MOTExpiryDate = vehicle.MOTExpiryDate,
                DateOfAscription = vehicle.DateOfAscription,
                Timeline = timeline
            };

            _dbContext.VehicleLookups.Add(vehicleLookup);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _maxInsertAmount--;
        }
        else if (_maxUpdateAmount != 0 && vehicleLookup != null)
        {
            timeline.AddRange(vehicleLookup.Timeline);
            vehicleLookup.LicensePlate = vehicle.LicensePlate;
            vehicleLookup.MOTExpiryDate = vehicle.MOTExpiryDate;
            vehicleLookup.DateOfAscription = vehicle.DateOfAscription;
            vehicleLookup.Timeline = timeline.OrderByDescending(x => x.Date).ToList();

            _dbContext.VehicleLookups.Update(vehicleLookup);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _maxUpdateAmount--;
        }
    }
}