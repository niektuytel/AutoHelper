using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;

public record UpsertVehicleLookupsCommand : IQueueRequest
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

    /// <summary>
    /// Only vehicles, not the carts and trailers
    /// </summary>
    public bool UpsertOnlyMOTRequiredVehicles { get; set; } = true;

    /// <summary>
    /// Check once a week if data is still up to date with RDW
    /// </summary>
    public DateTime RecentUpdateThreshold { get; set; } = DateTime.UtcNow.AddDays(-7);

    public IQueueService QueueService { get; set; }
}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<UpsertVehicleLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<UpsertVehicleLookupsCommandHandler> _logger;

    private int _maxInsertAmount { get; set; }
    private int _maxUpdateAmount { get; set; }

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService, ILogger<UpsertVehicleLookupsCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        _maxInsertAmount = request.MaxInsertAmount;
        _maxUpdateAmount = request.MaxUpdateAmount;

        if (_maxInsertAmount == -1)
        {
            request.QueueService.LogInformation($"Insert all available vehicles");
        }
        else
        {
            request.QueueService.LogInformation($"Insert {_maxInsertAmount} vehicles");
        }

        if (_maxUpdateAmount == -1)
        {
            request.QueueService.LogInformation($"Update all available vehicles");
        }
        else
        {
            request.QueueService.LogInformation($"Update {_maxUpdateAmount} vehicles");
        }

        var defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();
        try {
            await _vehicleService.ForEachVehicleBasicsInBatches(async vehicleBatch =>
            {
                var licensePlates = vehicleBatch.Select(v => v.LicensePlate).ToList();

                // Retrieve the required entities without join, using Contains and checking the LastModified date
                var vehicleLookupDictionary = await _dbContext.VehicleLookups
                    .Include(x => x.Timeline)
                    .Where(v => licensePlates.Contains(v.LicensePlate))
                    .ToDictionaryAsync(v => v.LicensePlate, cancellationToken);

                var updateTasks = vehicleBatch.Select(rdwVehicle => ProcessVehicleAsync(rdwVehicle, vehicleLookupDictionary, request, defectDescriptions, cancellationToken)).ToList();

                // After all parallel tasks are completed, we can now save the changes to the database.
                var vehicleLookupsToUpdate = new List<VehicleLookupItem>();
                var vehicleLookupsToInsert = new List<VehicleLookupItem>();
                var results = await Task.WhenAll(updateTasks);

                foreach (var result in results)
                {
                    if (result.Errors.Any())
                    {
                        foreach (var error in result.Errors)
                        {
                            request.QueueService.LogError(error);
                        }
                    }
                    else if (result.HasChanges)
                    {
                        result.VehicleLookup.Timeline = result.Timeline.OrderByDescending(x => x.Date).ToList();

                        if (_maxInsertAmount != 0 && !result.OnUpdate)
                        {
                            vehicleLookupsToInsert.Add(result.VehicleLookup);
                            _maxInsertAmount--;
                        }
                        else if (_maxUpdateAmount != 0 && result.OnUpdate)
                        {
                            vehicleLookupsToUpdate.Add(result.VehicleLookup);
                            _maxUpdateAmount--;
                        }
                        else if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                        {
                            break;
                        }
                    }
                }

                if (vehicleLookupsToInsert.Any())
                {
                    await _dbContext.BulkInsertAsync(vehicleLookupsToInsert, cancellationToken);
                    request.QueueService.LogInformation($"Insert {vehicleLookupsToInsert.Count} items");
                }

                if (vehicleLookupsToUpdate.Any())
                {
                    await _dbContext.BulkUpdateAsync(vehicleLookupsToUpdate, cancellationToken);
                    request.QueueService.LogInformation($"Update {vehicleLookupsToInsert.Count} items");
                }

                if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
                {
                    // cancel next operation
                    throw new OperationCanceledException();
                }
            }, request.UpsertOnlyMOTRequiredVehicles);
        }
        catch (OperationCanceledException ex)
        {
            request.QueueService.LogInformation($"Done, MaxInsertAmount:{_maxInsertAmount} MaxUpdateAmount:{_maxUpdateAmount} reached");
            return Unit.Value;
        }
        catch (Exception ex)
        {
            request.QueueService.LogError(ex.Message);
            throw;
        }   
  
        return Unit.Value;
    }

    async Task<(bool HasChanges, VehicleLookupItem VehicleLookup, List<VehicleTimelineItem> Timeline, bool OnUpdate, List<string> Errors)> ProcessVehicleAsync(
        RDWVehicleBasics vehicle,
        Dictionary<string, VehicleLookupItem> vehicleLookupDictionary,
        UpsertVehicleLookupsCommand request,
        IEnumerable<RDWDetectedDefectDescription> defectDescriptions,
        CancellationToken cancellationToken)
    {
        var onUpdate = vehicleLookupDictionary.TryGetValue(vehicle.LicensePlate, out var vehicleLookup);
        if (onUpdate == false)
        {
            vehicleLookup = new VehicleLookupItem
            {
                Id = Guid.NewGuid(),
                LicensePlate = vehicle.LicensePlate,
                DateOfMOTExpiry = vehicle.MOTExpiryDateDt,
                DateOfAscription = vehicle.RegistrationDateDt
            };
        }
        else
        {
            // still to recently to been updated
            if(vehicleLookup!.LastModified >= request.RecentUpdateThreshold)
            {
                return (false, vehicleLookup, vehicleLookup.Timeline ?? new List<VehicleTimelineItem>(), true, new List<string>());
            }

            vehicleLookup.Id = vehicleLookup.Id;
            vehicleLookup.DateOfMOTExpiry = vehicle.MOTExpiryDateDt;
            vehicleLookup.DateOfAscription = vehicle.RegistrationDateDt;
        }

        var hasChanges = false;
        var timeline = vehicleLookup.Timeline ?? new List<VehicleTimelineItem>();
        var errors = new List<string>();
        try
        {
            if (request.UpsertTimeline)
            {
                var updatedTimeline = await _vehicleService.GetVehicleUpdatedTimeline(timeline, vehicle, defectDescriptions);
                if (updatedTimeline?.Any() == true && updatedTimeline.Count() != timeline.Count())
                {
                    timeline = updatedTimeline;
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
            errors.Add($"[{vehicle.LicensePlate}]:{ex.Message}");
        }

        // Return a tuple with all the necessary info to apply changes later
        return (hasChanges, vehicleLookup, timeline, onUpdate, errors);
    }

}