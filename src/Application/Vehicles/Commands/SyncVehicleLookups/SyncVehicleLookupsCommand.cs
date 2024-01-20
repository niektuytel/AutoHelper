using System.Linq;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;

public record SyncVehicleLookupsCommand : IQueueRequest
{
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultDaysToCheck = -7;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public SyncVehicleLookupsCommand(
        int startRowIndex = DefaultStartingRowIndex,
        int endRowIndex = DefaultEndingRowIndex,
        int maxInsertAmount = InsertAll,
        int maxUpdateAmount = UpdateAll,
        int batchSize = 10000,
        DateTime? upsertOnlyLastModifiedOlderThan = null)
    {
        StartRowIndex = startRowIndex;
        EndRowIndex = endRowIndex;
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        BatchSize = batchSize;
        UpsertOnlyLastModifiedOlderThan = upsertOnlyLastModifiedOlderThan ?? DateTime.UtcNow.AddDays(DefaultDaysToCheck);
    }

    public int StartRowIndex { get; init; }
    public int EndRowIndex { get; set; }
    public int MaxInsertAmount { get; init; }
    public int MaxUpdateAmount { get; init; }
    public int BatchSize { get; set; }
    public DateTime UpsertOnlyLastModifiedOlderThan { get; init; }
    public IQueueService QueueService { get; set; }
}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<SyncVehicleLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IVehicleService _vehicleService;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _vehicleService = vehicleService;
    }

    public async Task<Unit> Handle(SyncVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        var totalAmountOfVehicles = await _vehicleService.GetVehicleBasicsWithMOTRequirementCount();
        _maxInsertAmount = request.MaxInsertAmount == SyncVehicleLookupsCommand.InsertAll ? totalAmountOfVehicles : Math.Min(request.MaxInsertAmount, totalAmountOfVehicles);
        _maxUpdateAmount = request.MaxUpdateAmount == SyncVehicleLookupsCommand.UpdateAll ? totalAmountOfVehicles : Math.Min(request.MaxUpdateAmount, totalAmountOfVehicles);

        // Offset to start from
        var limit = request.BatchSize;
        var offset = request.StartRowIndex > 0 ? request.StartRowIndex / limit : 0;
        var count = request.StartRowIndex > 0 ? request.StartRowIndex : 0;

        // set end row index to total amount of vehicles if not set
        if (request.EndRowIndex <= 0 || request.EndRowIndex >= totalAmountOfVehicles)
        {
            request.EndRowIndex = totalAmountOfVehicles;
        }

        LogInformationBasedOnAmount(request);

        do
        {
            if (ShouldStopProcessing(count, request, cancellationToken))
            {
                break;
            }

            var vehicleBatch = await _vehicleService.GetVehicleBasicsWithMOTRequirement(offset, limit);
            count += vehicleBatch.Count();
            offset++;

            var licensePlates = vehicleBatch.Select(x => x.LicensePlate).ToList();
            var vehicleLookups = _dbContext.VehicleLookups
                .Where(v => licensePlates.Contains(v.LicensePlate))
                .ToList();

            var (vehicleLookupsToInsert, vehicleLookupsToUpdate) = ProcessLookupsAsync(vehicleBatch, vehicleLookups, request);

            if (vehicleLookupsToInsert.Any())
            {
                await _dbContext.BulkInsertAsync(vehicleLookupsToInsert, cancellationToken);
            }

            if (vehicleLookupsToUpdate.Any())
            {
                await _dbContext.BulkUpdateAsync(vehicleLookupsToUpdate, cancellationToken);
            }

            var line = $"[{count}/{request.EndRowIndex}] insert: {vehicleLookupsToInsert.Count} | update: {vehicleLookupsToUpdate.Count} items";
            request.QueueService.LogInformation(line, inProgressBar: true);

        } while (count == limit * offset || count < request.EndRowIndex);

        request.QueueService.LogInformation($"Task finished");
        return Unit.Value;
    }

    private (List<VehicleLookupItem> Inserts, List<VehicleLookupItem> Updates) ProcessLookupsAsync(IEnumerable<VehicleBasicsDtoItem> vehicleBatch, IEnumerable<VehicleLookupItem> currentVehicleLookups, SyncVehicleLookupsCommand request)
    {
        var vehicleLookupsToUpdate = new List<VehicleLookupItem>();
        var vehicleLookupsToInsert = new List<VehicleLookupItem>();

        if (vehicleBatch?.Any() != true)
        {
            return (vehicleLookupsToInsert, vehicleLookupsToUpdate);
        }

        try
        {
            foreach (VehicleBasicsDtoItem vehicle in vehicleBatch)
            {
                var vehicleLookup = currentVehicleLookups.FirstOrDefault(v => v.LicensePlate == vehicle.LicensePlate);
                var onInsert = vehicleLookup == null;

                if (onInsert)
                {
                    vehicleLookup = _vehicleService.CreateVehicleRecord(vehicle);
                }
                else
                {
                    var hasChanges = _vehicleService.UpdateVehicleRecord(vehicle, vehicleLookup!, request.UpsertOnlyLastModifiedOlderThan);
                    if (hasChanges == false)
                    {
                        continue;
                    }
                }

                if (_maxInsertAmount > 0 && onInsert)
                {
                    vehicleLookupsToInsert.Add(vehicleLookup!);
                    _maxInsertAmount--;
                }
                else if (_maxUpdateAmount > 0 && !onInsert)
                {
                    vehicleLookupsToUpdate.Add(vehicleLookup!);
                    _maxUpdateAmount--;
                }

                if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            request.QueueService.LogError(ex.Message);
        }

        return (vehicleLookupsToInsert, vehicleLookupsToUpdate);
    }

    private void LogInformationBasedOnAmount(SyncVehicleLookupsCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == SyncVehicleLookupsCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available vehicle lookups");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} vehicle lookups");
        }

        if (request.MaxUpdateAmount == SyncVehicleLookupsCommand.UpdateAll)
        {
            request.QueueService.LogInformation($"Update all available vehicle lookups");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} vehicle lookups");
        }
    }

    private bool ShouldStopProcessing(int indexCount, SyncVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        if (indexCount >= request.EndRowIndex)
        {
            return true;
        }

        return HasReachedInsertLimit(request) && HasReachedUpdateLimit(request) || cancellationToken.IsCancellationRequested;
    }

    private bool HasReachedInsertLimit(SyncVehicleLookupsCommand request)
    {
        return request.MaxInsertAmount > 0 && _maxInsertAmount <= 0 || request.MaxInsertAmount == -1 && _maxInsertAmount <= 0;
    }

    private bool HasReachedUpdateLimit(SyncVehicleLookupsCommand request)
    {
        return request.MaxUpdateAmount > 0 && _maxUpdateAmount <= 0 || request.MaxUpdateAmount == -1 && _maxUpdateAmount <= 0;
    }

}