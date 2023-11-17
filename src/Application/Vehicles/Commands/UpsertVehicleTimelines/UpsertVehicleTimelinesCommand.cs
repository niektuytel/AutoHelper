using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;

public record UpsertVehicleTimelinesCommand : IQueueRequest
{
    // Constants for special handling cases like 'insert all' or 'update all'.
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultDaysToCheck = -7;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public UpsertVehicleTimelinesCommand()
    {
    }

    public UpsertVehicleTimelinesCommand(
        int startRowIndex = DefaultStartingRowIndex,
        int endRowIndex = DefaultEndingRowIndex,
        int maxInsertAmount = InsertAll,
        int maxUpdateAmount = UpdateAll,
        bool upsertTimeline = true,
        bool upsertServiceLogs = true,
        DateTime? upsertOnlyLastModifiedOlderThan = null)
    {
        StartRowIndex = startRowIndex;
        EndRowIndex = endRowIndex;
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        UpsertTimeline = upsertTimeline;
        UpsertServiceLogs = upsertServiceLogs;
        UpsertOnlyLastModifiedOlderThan = upsertOnlyLastModifiedOlderThan ?? DateTime.UtcNow.AddDays(DefaultDaysToCheck);
    }

    public int StartRowIndex { get; init; }
    public int EndRowIndex { get; set; }
    public int MaxInsertAmount { get; init; }
    public int MaxUpdateAmount { get; init; }
    public bool UpsertTimeline { get; init; }
    public bool UpsertServiceLogs { get; init; }
    public DateTime UpsertOnlyLastModifiedOlderThan { get; init; }
    public IQueueService QueueService { get; set; }
}

public class UpsertVehicleTimelinesCommandHandler : IRequestHandler<UpsertVehicleTimelinesCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<UpsertVehicleTimelinesCommandHandler> _logger;
    private IEnumerable<RDWDetectedDefectDescription> _defectDescriptions;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertVehicleTimelinesCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService, ILogger<UpsertVehicleTimelinesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpsertVehicleTimelinesCommand request, CancellationToken cancellationToken)
    {
        var vehicleLookups = await _dbContext.VehicleLookups
            .Where(x => x.LastModified < request.UpsertOnlyLastModifiedOlderThan)
            .ToDictionaryAsync(x => x.LicensePlate, x => new {
                x.DateOfMOTExpiry,
                x.DateOfAscription
            }, cancellationToken);
        
        _defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();
        _maxInsertAmount = request.MaxInsertAmount == UpsertVehicleTimelinesCommand.InsertAll ? vehicleLookups.Count : request.MaxInsertAmount;
        _maxUpdateAmount = request.MaxUpdateAmount == UpsertVehicleTimelinesCommand.UpdateAll ? vehicleLookups.Count : request.MaxUpdateAmount;

        var limit = 1000;
        var offset = 0;
        var count = 0;

        // set offset to start row index if set
        if (request.StartRowIndex > 0)
        {
            offset = request.StartRowIndex / limit;
            count = request.StartRowIndex;
        }

        // set end row index to total amount of vehicles if not set
        if (request.EndRowIndex <= 0)
        {
            request.EndRowIndex = vehicleLookups.Count;
        }

        LogInformationBasedOnAmount(request);


        // TODO: He is to slow on inserting when have already insert 8.5million vehicles
        // TODO: Get all existing vehicle lookup licensePlates
        // TODO: Get all stored vehicle lookup licensePlates

        //do
        //{
        //    //// TODO: if only insert than ignore existing lookup licensePlates
        //    //// TODO: if only update than ignore non existing lookup licensePlates

        //    //var vehicleBatch = await _vehicleService.GetVehicleBasicsWithMOTRequirement(offset, limit);
        //    //count += vehicleBatch.Count();
        //    //offset++;

        //    //if (_maxUpdateAmount == 0)
        //    //{
        //    //    // only unknown license plates are allowed
        //    //    vehicleBatch = vehicleBatch.Where(x => !licensePlates.ContainsKey(x.LicensePlate));
        //    //}

        //    ////if (!vehicleBatch.Any())
        //    ////{
        //    ////    request.QueueService.LogInformation($"No vehicles found to process.");
        //    ////    break;
        //    ////}


        //    //var (vehicleLookupsToInsert, vehicleLookupsToUpdate, vehicleTimelineItemsToInsert) = await ProcessVehicleBatchAsync(vehicleBatch, request, cancellationToken);

        //    //if (vehicleLookupsToInsert.Any())
        //    //{
        //    //    await _dbContext.BulkInsertAsync(vehicleLookupsToInsert, cancellationToken);
        //    //}

        //    //if (vehicleLookupsToUpdate.Any())
        //    //{
        //    //    await _dbContext.BulkUpdateAsync(vehicleLookupsToUpdate, cancellationToken);
        //    //}

        //    //if (vehicleTimelineItemsToInsert.Any())
        //    //{
        //    //    await _dbContext.BulkInsertAsync(vehicleTimelineItemsToInsert, cancellationToken);
        //    //}

        //    //request.QueueService.LogInformation($"[{count}/{request.EndRowIndex}] " +
        //    //    $"Lookups (insert: {vehicleLookupsToInsert.Count} | update: {vehicleLookupsToUpdate.Count}) " +
        //    //    $"Timelines (insert: {vehicleTimelineItemsToInsert.Count})"
        //    //);

        //    //if (_maxInsertAmount == 0 && _maxUpdateAmount == 0 || cancellationToken.IsCancellationRequested)
        //    //{
        //    //    request.QueueService.LogInformation($"Operation finished.");
        //    //    break;
        //    //}
        //} while (count == limit * offset || count <= request.EndRowIndex);

        request.QueueService.LogInformation($"Done processing. Inserted: {request.MaxInsertAmount - _maxInsertAmount}, Updated: {request.MaxUpdateAmount - _maxUpdateAmount}");
        return Unit.Value;
    }

    private void LogInformationBasedOnAmount(UpsertVehicleTimelinesCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == UpsertVehicleTimelinesCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available vehicles");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} vehicles");
        }

        if (request.MaxUpdateAmount == UpsertVehicleTimelinesCommand.UpdateAll)
        {
            request.QueueService.LogInformation($"Update all available vehicles");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} vehicles");
        }
    }

    private async Task<(List<VehicleLookupItem> Inserts, List<VehicleLookupItem> Updates, List<VehicleTimelineItem> InsertTimelineItems)> ProcessVehicleBatchAsync(
        IEnumerable<RDWVehicleBasics> vehicleBatch,
        UpsertVehicleTimelinesCommand request,
        CancellationToken cancellationToken)
    {

        var vehicleLookupsToUpdate = new List<VehicleLookupItem>();
        var vehicleLookupsToInsert = new List<VehicleLookupItem>();
        var vehicleTimelinesToInsert = new List<VehicleTimelineItem>();
        var licensePlates = vehicleBatch.Select(v => v.LicensePlate).ToList();
        try
        {
            var vehicleLookups = await _dbContext.VehicleLookups
                .Include(x => x.Timeline)
                .Where(v => licensePlates.Contains(v.LicensePlate))
                .ToDictionaryAsync(v => v.LicensePlate, cancellationToken);

            var defectsBatch = await _vehicleService.GetVehicleDetectedDefects(licensePlates);
            var inspectionsBatch = await _vehicleService.GetVehicleInspectionNotifications(licensePlates);
            foreach (var vehicle in vehicleBatch)
            {
                var onUpdate = vehicleLookups.TryGetValue(vehicle.LicensePlate, out var vehicleLookup);
                var vehicleTimelineToInsert = new List<VehicleTimelineItem>();
                if (onUpdate)
                {
                    if (vehicleLookup!.LastModified >= request.UpsertOnlyLastModifiedOlderThan)
                    {
                        continue;
                    }

                    vehicleLookup.DateOfMOTExpiry = vehicle.MOTExpiryDateDt;
                    vehicleLookup.DateOfAscription = vehicle.RegistrationDateDt;
                    vehicleLookup.LastModified = DateTime.UtcNow;
                    vehicleLookup.LastModifiedBy = $"system";
                }
                else
                {
                    vehicleLookup = new VehicleLookupItem
                    {
                        LicensePlate = vehicle.LicensePlate,
                        DateOfMOTExpiry = vehicle.MOTExpiryDateDt,
                        DateOfAscription = vehicle.RegistrationDateDt,
                        Created = DateTime.UtcNow,
                        CreatedBy = $"system",
                        LastModified = DateTime.UtcNow,
                        LastModifiedBy = $"system"
                    };
                }

                var hasChanges = false;
                try
                {
                    if (request.UpsertTimeline)
                    {
                        var defects = defectsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
                        if (defects?.Any() == true)
                        {
                            var failedMOTs = _vehicleService.UndefinedFailedMOTTimelineItems(vehicle.LicensePlate, vehicleLookup.Timeline, defects, _defectDescriptions);
                            if (failedMOTs?.Any() == true)
                            {
                                vehicleTimelineToInsert.AddRange(failedMOTs);
                                hasChanges = true;
                            }
                        }

                        var inspections = inspectionsBatch!.Where(x => x.LicensePlate == vehicle.LicensePlate);
                        if (inspections?.Any() == true)
                        {
                            var succeededMOTs = _vehicleService.UndefinedSucceededMOTTimelineItems(vehicle.LicensePlate, vehicleLookup.Timeline, inspections);
                            if (succeededMOTs?.Any() == true)
                            {
                                vehicleTimelineToInsert.AddRange(succeededMOTs);
                                hasChanges = true;
                            }
                        }

                        var ownerChanged = _vehicleService.UndefinedOwnerChangedTimelineItem(vehicle.LicensePlate, vehicleLookup.Timeline, vehicle.RegistrationDateDt);
                        if (ownerChanged != null)
                        {
                            vehicleTimelineToInsert.Add(ownerChanged);
                            hasChanges = true;
                        }
                    }

                    if (request.UpsertServiceLogs)
                    {
                        // TODO: implement
                        throw new NotImplementedException();
                        hasChanges = true;
                    }
                }
                catch (Exception ex)
                {
                    request.QueueService.LogError($"[{vehicle.LicensePlate}]:{ex.Message}");
                }

                if (hasChanges)
                {
                    if (_maxInsertAmount != 0 && !onUpdate)
                    {
                        vehicleLookupsToInsert.Add(vehicleLookup);
                        vehicleTimelinesToInsert.AddRange(vehicleTimelineToInsert);
                        _maxInsertAmount--;
                    }
                    else if (_maxUpdateAmount != 0 && onUpdate)
                    {
                        vehicleLookupsToUpdate.Add(vehicleLookup);
                        vehicleTimelinesToInsert.AddRange(vehicleTimelineToInsert);
                        _maxUpdateAmount--;
                    }
                    else if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                    {
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            request.QueueService.LogError(ex.Message);
        }

        return (vehicleLookupsToInsert, vehicleLookupsToUpdate, vehicleTimelinesToInsert);
    }

}