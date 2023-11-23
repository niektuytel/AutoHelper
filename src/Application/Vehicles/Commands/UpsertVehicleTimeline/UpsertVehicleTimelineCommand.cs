﻿using System.Collections.Generic;
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

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimeline;

public record UpsertVehicleTimelineCommand : IRequest<string>
{
    public UpsertVehicleTimelineCommand()
    {
    }

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

    public async Task<string> Handle(UpsertVehicleTimelineCommand request, CancellationToken cancellationToken)
    {
        _defectDescriptions = await _vehicleService.GetDetectedDefectDescriptionsAsync();

        var batch = await _dbContext.VehicleLookups
            .Where(x => x.LicensePlate == request.LicensePlate)
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

        return $"insert: {vehicleTimelineItemsToInsert.Count} | update: {vehicleTimelineItemsToUpdate.Count} items";
    }

    private async Task<(List<VehicleTimelineItem> InsertTimelineItems, List<VehicleTimelineItem> UpdateTimelineItems)> ProcessVehicleBatchAsync(
        Dictionary<string, VehicleLookupItem> batch,
        UpsertVehicleTimelineCommand request,
        CancellationToken cancellationToken)
    {
        var licensePlates = batch.Keys.ToList();
        var defectsBatch = await _vehicleService.GetVehicleDetectedDefects(licensePlates);
        var inspectionsBatch = await _vehicleService.GetVehicleInspectionNotifications(licensePlates);
        var serviceLogsBatch = await _dbContext.VehicleServiceLogs
            .Where(x => x.VehicleLicensePlate == request.LicensePlate)
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
                _logger.LogError($"[{vehicle.Key}]:{ex.Message}");
            }
        }

        return (vehicleTimelinesToInsert, vehicleTimelinesToUpdate);
    }

}