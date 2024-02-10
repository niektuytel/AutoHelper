using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleTimeline;

public record SyncVehicleTimelineCommand : IRequest<string>
{
    public SyncVehicleTimelineCommand(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }

}

public class UpsertVehicleTimelinesCommandHandler : IRequestHandler<SyncVehicleTimelineCommand, string>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;
    private readonly IVehicleTimelineService _vehicleTimelineService;
    private readonly ILogger<UpsertVehicleTimelinesCommandHandler> _logger;
    private IEnumerable<VehicleDetectedDefectDescriptionDtoItem> _defectDescriptions;

    public UpsertVehicleTimelinesCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService, IVehicleTimelineService vehicleTimelineService, ILogger<UpsertVehicleTimelinesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
        _vehicleTimelineService = vehicleTimelineService;
        _logger = logger;
    }

    public async Task<string> Handle(SyncVehicleTimelineCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.VehicleLookups
            .Include(x => x.Timeline)
            .FirstOrDefaultAsync(x => x.LicensePlate == request.LicensePlate, cancellationToken);

        if (vehicle == null)
        {
            return "Vehicle not found";
        }

        _defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();
        var defectsBatch = await _vehicleService.GetVehicleDetectedDefects(new() { request.LicensePlate });
        var inspectionsBatch = await _vehicleService.GetVehicleInspectionNotifications(new() { request.LicensePlate });
        var serviceLogsBatch = await _dbContext.VehicleServiceLogs
            .Where(x => x.VehicleLicensePlate == request.LicensePlate)
            .ToListAsync(cancellationToken);

        var vehicleTimelinesToInsert = new List<VehicleTimelineItem>();
        var vehicleTimelinesToUpdate = new List<VehicleTimelineItem>();
        try
        {
            var itemsToInsert = await _vehicleTimelineService.InsertableTimelineItems(
                vehicle,
                defectsBatch,
                inspectionsBatch,
                serviceLogsBatch,
                _defectDescriptions
            );

            if (itemsToInsert.Any() == true)
            {
                await _dbContext.BulkInsertAsync(itemsToInsert, cancellationToken);
            }

            return $"insert: {itemsToInsert.Count} | update: 0 items";
        }
        catch (Exception ex)
        {
            var message = $"[{request.LicensePlate}]:{ex.Message}";

            _logger.LogError(message);
            return message;
        }
    }

}