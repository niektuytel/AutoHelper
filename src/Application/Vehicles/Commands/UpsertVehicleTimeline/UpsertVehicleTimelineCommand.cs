using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimeline;

public record UpsertVehicleTimelineCommand : IRequest<string>
{
    public UpsertVehicleTimelineCommand(string licensePlate)
    {
        licensePlate = licensePlate.ToUpper().Replace(" ", "").Replace("-", "");
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }

}

public class UpsertVehicleTimelinesCommandHandler : IRequestHandler<UpsertVehicleTimelineCommand, string>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;
    private readonly ILogger<UpsertVehicleTimelinesCommandHandler> _logger;
    private IEnumerable<VehicleDetectedDefectDescriptionDtoItem> _defectDescriptions;
    private int _maxInsertAmount;
    private int _maxUpdateAmount;

    public UpsertVehicleTimelinesCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService, ILogger<UpsertVehicleTimelinesCommandHandler> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
        _logger = logger;
    }

    public async Task<string> Handle(UpsertVehicleTimelineCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _dbContext.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == request.LicensePlate, cancellationToken);
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
            var (itemsToInsert, itemsToUpdate) = await _vehicleService.UpsertTimelineItems(
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

            return $"insert: {itemsToInsert.Count} | update: {itemsToUpdate.Count} items";
        }
        catch (Exception ex)
        {
            var message = $"[{request.LicensePlate}]:{ex.Message}";

            _logger.LogError(message);
            return message;
        }
    }

}