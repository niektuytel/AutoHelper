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

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;

public record UpsertVehicleLookupsCommand : IQueueRequest
{
    public const int InsertAll = -1;
    public const int UpdateAll = -1;
    public const int DefaultDaysToCheck = -7;
    public const int DefaultStartingRowIndex = 0;
    public const int DefaultEndingRowIndex = -1;

    public UpsertVehicleLookupsCommand()
    {
    }

    public UpsertVehicleLookupsCommand(
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

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<UpsertVehicleLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<UpsertVehicleLookupsCommandHandler> _logger;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService, ILogger<UpsertVehicleLookupsCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        var totalAmountOfVehicles = await _vehicleService.GetVehicleBasicsWithMOTRequirementCount();
        _maxInsertAmount = request.MaxInsertAmount == UpsertVehicleLookupsCommand.InsertAll ? totalAmountOfVehicles : request.MaxInsertAmount;
        _maxUpdateAmount = request.MaxUpdateAmount == UpsertVehicleLookupsCommand.UpdateAll ? totalAmountOfVehicles : request.MaxUpdateAmount;

        // Offset to start from
        var limit = request.BatchSize;
        var offset = (request.StartRowIndex > 0) ? (request.StartRowIndex / limit) : 0;
        var count = (request.StartRowIndex > 0) ? request.StartRowIndex : 0;

        // set end row index to total amount of vehicles if not set
        if (request.EndRowIndex <= 0)
        {
            request.EndRowIndex = totalAmountOfVehicles;
        }

        LogInformationBasedOnAmount(request);

        var licensePlates = _dbContext.VehicleLookups
            .Select(x => x.LicensePlate)
            .ToDictionary(x => x, x => new Tuple<bool>(true));

        do
        {
            if (_maxInsertAmount == 0 && _maxUpdateAmount == 0 || cancellationToken.IsCancellationRequested)
            {
                request.QueueService.LogInformation($"Operation finished.");
                break;
            }

            var vehicleBatch = await _vehicleService.GetVehicleBasicsWithMOTRequirement(offset, limit);
            count += vehicleBatch.Count();
            offset++;

            if(_maxUpdateAmount == 0)
            {
                // only unknown license plates are allowed
                vehicleBatch = vehicleBatch.Where(x => licensePlates.ContainsKey(x.LicensePlate) == false);
            }

            if(_maxInsertAmount == 0)
            {
                // only known license plates are allowed
                vehicleBatch = vehicleBatch.Where(x => licensePlates.ContainsKey(x.LicensePlate) == true);
            }
            
            if (vehicleBatch.Any())
            {
                var (vehicleLookupsToInsert, vehicleLookupsToUpdate) = await ProcessLookupsAsync(vehicleBatch, request, cancellationToken);
                if (vehicleLookupsToInsert.Any())
                {
                    await _dbContext.BulkInsertAsync(vehicleLookupsToInsert, cancellationToken);
                }

                if (vehicleLookupsToUpdate.Any())
                {
                    await _dbContext.BulkUpdateAsync(vehicleLookupsToUpdate, cancellationToken);
                }

                request.QueueService.LogInformation(
                    $"[{count}/{request.EndRowIndex}] insert: {vehicleLookupsToInsert.Count} | update: {vehicleLookupsToUpdate.Count} items"
                );
            }
            else
            {
                request.QueueService.LogInformation(
                    $"[{count}/{request.EndRowIndex}] insert: 0 | update: 0 items"
                );
            }
        } while (count == (limit * offset) || count < request.EndRowIndex);

        request.QueueService.LogInformation($"Done processing. Inserted: {request.MaxInsertAmount - _maxInsertAmount}, Updated: {request.MaxUpdateAmount - _maxUpdateAmount}");
        return Unit.Value;
    }

    private async Task<(List<VehicleLookupItem> Inserts, List<VehicleLookupItem> Updates)> ProcessLookupsAsync(IEnumerable<RDWVehicleBasics> vehicleBatch, UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        var vehicleLookupsToUpdate = new List<VehicleLookupItem>();
        var vehicleLookupsToInsert = new List<VehicleLookupItem>();
        try
        {
            var licensePlates = vehicleBatch.Select(v => v.LicensePlate).ToList();
            var vehicleLookups = await _dbContext.VehicleLookups
                .Where(v => licensePlates.Contains(v.LicensePlate))
                .ToDictionaryAsync(v => v.LicensePlate, cancellationToken);

            foreach (var vehicle in vehicleBatch)
            {
                var onUpdate = vehicleLookups.TryGetValue(vehicle.LicensePlate, out var vehicleLookup);
                if (onUpdate)
                {
                    // only update when something has changed
                    var somethingChanged = HasChanges(vehicleLookup, vehicle, request.UpsertOnlyLastModifiedOlderThan);
                    if (!somethingChanged)
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

                if (_maxInsertAmount != 0 && !onUpdate)
                {
                    vehicleLookupsToInsert.Add(vehicleLookup);
                    _maxInsertAmount--;
                }
                else if (_maxUpdateAmount != 0 && onUpdate)
                {
                    vehicleLookupsToUpdate.Add(vehicleLookup);
                    _maxUpdateAmount--;
                }
                else if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
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

    private bool HasChanges(VehicleLookupItem? vehicleLookup, RDWVehicleBasics vehicle, DateTime upsertOnlyLastModifiedOlderThan)
    {
        if (vehicleLookup == null)
        {
            return false;
        }

        var sameExpirationDate = vehicleLookup.DateOfMOTExpiry == vehicle.MOTExpiryDateDt;
        var sameRegistrationDate = vehicleLookup.DateOfAscription == vehicle.RegistrationDateDt;
        if (vehicleLookup!.LastModified >= upsertOnlyLastModifiedOlderThan)
        {
            return false;
        }
        else if (sameExpirationDate && sameRegistrationDate)
        {
            return false;
        }

        return true;
    }

    private void LogInformationBasedOnAmount(UpsertVehicleLookupsCommand request)
    {
        request.QueueService.LogInformation($"Start upsert rows from {request.StartRowIndex} to {request.EndRowIndex}");

        if (request.MaxInsertAmount == UpsertVehicleLookupsCommand.InsertAll)
        {
            request.QueueService.LogInformation($"Insert all available vehicle lookups");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} vehicle lookups");
        }

        if (request.MaxUpdateAmount == UpsertVehicleLookupsCommand.UpdateAll)
        {
            request.QueueService.LogInformation($"Update all available vehicle lookups");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} vehicle lookups");
        }
    }

}