using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;

public record SyncVehicleTimelinesCommand : IQueueRequest<Unit>
{
    // Constants for special handling cases like 'insert all' or 'update all'.
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultDaysToCheck = -7;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public SyncVehicleTimelinesCommand()
    {
    }

    public SyncVehicleTimelinesCommand(
        int startRowIndex = DefaultStartingRowIndex,
        int endRowIndex = DefaultEndingRowIndex,
        int maxInsertAmount = InsertAll,
        int maxUpdateAmount = UpdateAll,
        int batchSize = 1000
    )
    {
        StartRowIndex = startRowIndex;
        EndRowIndex = endRowIndex;
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        BatchSize = batchSize;
    }

    public int StartRowIndex { get; init; }
    public int EndRowIndex { get; set; }
    public int MaxInsertAmount { get; init; }
    public int MaxUpdateAmount { get; init; }
    public int BatchSize { get; init; }

    [JsonIgnore]
    public IQueueContext QueueingService { get; set; }
}

public class UpsertVehicleTimelinesCommandHandler : IRequestHandler<SyncVehicleTimelinesCommand, Unit>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IVehicleService _vehicleService;
    private readonly IVehicleTimelineService _vehicleTimelineService;
    private IEnumerable<VehicleDetectedDefectDescriptionDtoItem> _defectDescriptions;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertVehicleTimelinesCommandHandler(IApplicationDbContext dbContext, IVehicleService vehicleService, IVehicleTimelineService vehicleTimelineService)
    {
        _dbContext = dbContext;
        _vehicleService = vehicleService;
        _vehicleTimelineService = vehicleTimelineService;
    }

    public async Task<Unit> Handle(SyncVehicleTimelinesCommand request, CancellationToken cancellationToken)
    {
        int totalVehicleRecords = await CalculateTotalRecords(request, cancellationToken);
        SetMaxInsertAndUpdateAmounts(request, totalVehicleRecords);

        _defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();
        LogInformationBasedOnAmount(request);

        int numberOfBatches = CalculateNumberOfBatches(request.BatchSize, totalVehicleRecords);
        for (int i = 0; i < numberOfBatches; i++)
        {
            var start = request.StartRowIndex + (i * request.BatchSize);
            if (ShouldStopProcessing(start, request, cancellationToken))
            {
                break;
            }

            var batch = await _dbContext.VehicleLookups
                .Include(x => x.Timeline)
                .OrderBy(x => x.LicensePlate) // Ensure a consistent order for paging
                .Skip(start)
                .Take(request.BatchSize)
                .ToDictionaryAsync(x => x.LicensePlate, x => x, cancellationToken);

            var (vehicleTimelineItemsToInsert, vehicleTimelineItemsToUpdate) = await ProcessVehicleBatchAsync(batch, request, cancellationToken);

            if (vehicleTimelineItemsToInsert.Any())
            {
                await _dbContext.BulkInsertAsync(vehicleTimelineItemsToInsert, cancellationToken);
            }

            if (vehicleTimelineItemsToUpdate.Any())
            {
                await _dbContext.BulkUpdateAsync(vehicleTimelineItemsToUpdate, cancellationToken);
            }

            var line = $"[{start + request.BatchSize}/{request.EndRowIndex}] insert: {vehicleTimelineItemsToInsert.Count} | update: {vehicleTimelineItemsToUpdate.Count} items";
            request.QueueingService.LogInformation(line, inProgressBar: true);

        }

        var message = $"Task finished.";
        request.QueueingService.LogInformation(message);
        return Unit.Value;
    }

    private async Task<int> CalculateTotalRecords(SyncVehicleTimelinesCommand request, CancellationToken cancellationToken)
    {
        int totalRecords = await _dbContext.VehicleLookups.CountAsync(cancellationToken);

        // get only omount from start to end row index
        if (request.EndRowIndex > 0)
        {
            totalRecords = request.EndRowIndex - request.StartRowIndex;
        }
        else
        {
            request.EndRowIndex = totalRecords;
        }

        return totalRecords;
    }

    private void SetMaxInsertAndUpdateAmounts(SyncVehicleTimelinesCommand request, int totalAmountOfVehicles)
    {
        _maxInsertAmount = request.MaxInsertAmount == SyncVehicleTimelinesCommand.InsertAll ? totalAmountOfVehicles : Math.Min(request.MaxInsertAmount, totalAmountOfVehicles);
        _maxUpdateAmount = request.MaxUpdateAmount == SyncVehicleTimelinesCommand.UpdateAll ? totalAmountOfVehicles : Math.Min(request.MaxUpdateAmount, totalAmountOfVehicles);
    }

    private int CalculateNumberOfBatches(int batchSize, int totalRecords)
    {
        return (totalRecords / batchSize) + (totalRecords % batchSize > 0 ? 1 : 0);
    }

    private async Task<(List<VehicleTimelineItem> InsertTimelineItems, List<VehicleTimelineItem> UpdateTimelineItems)> ProcessVehicleBatchAsync(Dictionary<string, VehicleLookupItem> batch, SyncVehicleTimelinesCommand request, CancellationToken cancellationToken)
    {
        var licensePlates = batch.Keys.ToList();
        var defectsBatch = await _vehicleService.GetVehicleDetectedDefects(licensePlates);
        var inspectionsBatch = await _vehicleService.GetVehicleInspectionNotifications(licensePlates);
        var serviceLogsBatch = await _dbContext.VehicleServiceLogs
            .Where(x => licensePlates.Contains(x.VehicleLicensePlate))
            .ToListAsync(cancellationToken);

        var vehicleTimelinesToInsert = new List<VehicleTimelineItem>();
        var vehicleTimelinesToUpdate = new List<VehicleTimelineItem>();
        foreach (var vehicle in batch)
        {
            try
            {
                var itemsToInsert = await _vehicleTimelineService.InsertableTimelineItems(
                    vehicle.Value,
                    defectsBatch,
                    inspectionsBatch,
                    serviceLogsBatch,
                    _defectDescriptions
                );

                if (_maxInsertAmount > 0 && itemsToInsert?.Any() == true)
                {
                    vehicleTimelinesToInsert.AddRange(itemsToInsert);

                    // insert on 1 vehicle, insert amount is based on the amount of vehicles we insert timelines for.
                    _maxInsertAmount--;
                }

                if (_maxInsertAmount <= 0 && _maxUpdateAmount <= 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                request.QueueingService.LogError($"[{vehicle.Key}]:{ex.Message}");
            }
        }

        return (vehicleTimelinesToInsert, vehicleTimelinesToUpdate);
    }

    private void LogInformationBasedOnAmount(SyncVehicleTimelinesCommand request)
    {
        request.QueueingService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == SyncVehicleTimelinesCommand.InsertAll)
        {
            request.QueueingService.LogInformation($"Insert all available vehicle timelines");
        }
        else
        {
            request.QueueingService.LogInformation($"Insert {_maxInsertAmount} vehicle timelines");
        }

        if (request.MaxUpdateAmount == SyncVehicleTimelinesCommand.UpdateAll)
        {
            request.QueueingService.LogInformation($"Update all available vehicle timelines");
        }
        else
        {
            request.QueueingService.LogInformation($"Update {_maxUpdateAmount} vehicle timelines");
        }
    }

    private bool ShouldStopProcessing(int startIndex, SyncVehicleTimelinesCommand request, CancellationToken cancellationToken)
    {
        if (startIndex >= request.EndRowIndex)
        {
            return true;
        }

        return HasReachedInsertLimit(request) && HasReachedUpdateLimit(request) || cancellationToken.IsCancellationRequested;
    }

    private bool HasReachedInsertLimit(SyncVehicleTimelinesCommand request)
    {
        return request.MaxInsertAmount > 0 && _maxInsertAmount <= 0 || request.MaxInsertAmount == -1 && _maxInsertAmount <= 0;
    }

    private bool HasReachedUpdateLimit(SyncVehicleTimelinesCommand request)
    {
        return request.MaxUpdateAmount > 0 && _maxUpdateAmount <= 0 || request.MaxUpdateAmount == -1 && _maxUpdateAmount <= 0;
    }

}