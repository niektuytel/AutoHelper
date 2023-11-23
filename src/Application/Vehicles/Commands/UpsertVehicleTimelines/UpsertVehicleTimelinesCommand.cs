using System.Collections.Generic;
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
        // Offset to start from
        var batchSize = request.BatchSize;
        var offset = (request.StartRowIndex > 0) ? (request.StartRowIndex / batchSize) : 0;
        int totalRecords = await _dbContext.VehicleLookups.CountAsync(cancellationToken);
        if (request.EndRowIndex > 0)
        {
            totalRecords = request.EndRowIndex - request.StartRowIndex;
        }
        else
        {
            request.EndRowIndex = totalRecords;
        }

        _maxInsertAmount = request.MaxInsertAmount == UpsertVehicleTimelinesCommand.InsertAll ? totalRecords : request.MaxInsertAmount;
        _maxUpdateAmount = request.MaxUpdateAmount == UpsertVehicleTimelinesCommand.UpdateAll ? totalRecords : request.MaxUpdateAmount;
        _defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();

        LogInformationBasedOnAmount(request);

        int numberOfBatches = (totalRecords / batchSize) + (totalRecords % batchSize == 0 ? 0 : 1);
        for (int i = 0; i < numberOfBatches; i++)
        {
            var catchLimitToInsert = (request.MaxInsertAmount > 0 && _maxInsertAmount <= 0) || (request.MaxInsertAmount == -1 && _maxInsertAmount == 0);
            var catchLimitToUpdate = (request.MaxUpdateAmount > 0 && _maxUpdateAmount <= 0) || (request.MaxUpdateAmount == -1 && _maxUpdateAmount == 0);
            if (catchLimitToInsert && catchLimitToUpdate || cancellationToken.IsCancellationRequested)
            {
                break;
            }

            var start = request.StartRowIndex + (i * batchSize);
            var batch = await _dbContext.VehicleLookups
                .Include(x => x.Timeline)
                .OrderBy(x => x.LicensePlate) // Ensure a consistent order for paging
                .Skip(start)
                .Take(batchSize)
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

            request.QueueService.LogInformation(
                $"[{(start + batchSize)}/{request.EndRowIndex}] insert: {vehicleTimelineItemsToInsert.Count} | update: {vehicleTimelineItemsToUpdate.Count} items"
            );
        }

        request.QueueService.LogInformation($"Operation finished. Inserted: {request.MaxInsertAmount - _maxInsertAmount}, Updated: {request.MaxUpdateAmount - _maxUpdateAmount}");
        return Unit.Value;
    }


    private async Task<(List<VehicleTimelineItem> InsertTimelineItems, List<VehicleTimelineItem> UpdateTimelineItems)> ProcessVehicleBatchAsync(
        Dictionary<string, VehicleLookupItem> batch,
        UpsertVehicleTimelinesCommand request,
        CancellationToken cancellationToken)
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
                var (itemsToInsert, _) = await _vehicleService.UpsertTimelineItems(
                    vehicle.Value,
                    defectsBatch,
                    inspectionsBatch,
                    serviceLogsBatch,
                    _defectDescriptions
                );

                if (itemsToInsert?.Any() == true)
                {
                    vehicleTimelinesToInsert.AddRange(itemsToInsert);
                    _maxInsertAmount -= itemsToInsert.Count;
                }

                if (_maxInsertAmount <= 0 && _maxUpdateAmount <= 0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                request.QueueService.LogError($"[{vehicle.Key}]:{ex.Message}");
            }
        }

        return (vehicleTimelinesToInsert, vehicleTimelinesToUpdate);
    }

    private void LogInformationBasedOnAmount(UpsertVehicleTimelinesCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == UpsertVehicleTimelinesCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available vehicle timelines");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} vehicle timelines");
        }

        if (request.MaxUpdateAmount == UpsertVehicleTimelinesCommand.UpdateAll)
        {
            request.QueueService.LogInformation($"Update all available vehicle timelines");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} vehicle timelines");
        }
    }

}