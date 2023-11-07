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
            await _vehicleService.ForEachVehicleInBatches(async vehicleBatch =>
            {
                foreach (var vehicle in vehicleBatch)
                {
                    var vehicleLookup = await _dbContext.VehicleLookups
                        .Include(x => x.Timeline)
                        .FirstOrDefaultAsync(x => x.LicensePlate == vehicle.LicensePlate, cancellationToken);

                    // always be sure that the vehicle lookup is up-to-date
                    var onInsert = vehicleLookup == null;
                    if (onInsert)
                    {
                        vehicleLookup = new VehicleLookupItem
                        {
                            LicensePlate = vehicle.LicensePlate
                        };
                    }
                    vehicleLookup!.DateOfMOTExpiry = vehicle.MOTExpiryDateDt;
                    vehicleLookup.DateOfAscription = vehicle.RegistrationDateDt;

                    var hasChanges = false;
                    var timeline = vehicleLookup.Timeline ?? new List<VehicleTimelineItem>();
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

                    if (hasChanges)
                    {
                        await UpsertVehicleLookup(vehicleLookup, timeline, onInsert, cancellationToken);

                        // Calculate the progress
                        double totalOperations = request.MaxInsertAmount + request.MaxUpdateAmount;
                        double completedOperations = (request.MaxInsertAmount - _maxInsertAmount) + (request.MaxUpdateAmount - _maxUpdateAmount);
                        int progressPercentage = (int)Math.Round((completedOperations / totalOperations) * 100);
                        if(progressPercentage >= 100) progressPercentage = 1; // Ensure it doesn't exceed 100%
                        request.QueueService.LogProgress(progressPercentage);
                    }
                }
            });
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

    private async Task UpsertVehicleLookup(
        VehicleLookupItem vehicleLookup, 
        List<VehicleTimelineItem> timeline, 
        bool onInsert, 
        CancellationToken cancellationToken
    ){
        vehicleLookup.Timeline = timeline.OrderByDescending(x => x.Date).ToList();

        if (_maxInsertAmount != 0 && onInsert)
        {
            _dbContext.VehicleLookups.Add(vehicleLookup);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _maxInsertAmount--;
        }
        else if (_maxUpdateAmount != 0 && !onInsert)
        {
            _dbContext.VehicleLookups.Update(vehicleLookup);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _maxUpdateAmount--;
        }

        if (_maxInsertAmount == 0 && _maxUpdateAmount == 0)
        {
            // cancel next operation
            throw new OperationCanceledException();
        }
    }
}