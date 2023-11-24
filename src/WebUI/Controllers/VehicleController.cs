﻿using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLog;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookupByReporter;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimeline;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Hangfire.MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
using WebUI.Models;
using YamlDotNet.Core.Tokens;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecifications;
using System;

namespace AutoHelper.WebUI.Controllers;

/// <summary>
/// Manages vehicle-related operations such as retrieving vehicle specifications, service logs, and timelines, as well as updating vehicle data.
/// </summary>
public class VehicleController : ApiControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet($"{nameof(GetSpecificationsCard)}")]
    [ProducesResponseType(typeof(VehicleSpecificationsCardItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecificationsCardItem> GetSpecificationsCard([FromQuery] string licensePlate)
    {
        // TODO: advice on service of the car at given mileage
        return await Mediator.Send(new GetVehicleSpecificationsCardQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecificationsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecificationsDtoItem> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecificationsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(VehicleServiceLogDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogDtoItem[]> GetServiceLogs([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleServiceLogsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetTimeline)}")]
    [ProducesResponseType(typeof(VehicleTimelineDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleTimelineDtoItem[]> GetTimeline([FromQuery] string licensePlate, [FromQuery] int maxAmount=5)
    {
        return await Mediator.Send(new GetVehicleTimelineQuery(licensePlate, maxAmount));
    }

    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [Authorize]// TODO: (Policy="Admin")
    [HttpPut($"{nameof(UpsertLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertLookups(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 10000
    )
    {
        var command = new UpsertVehicleLookupsCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(UpsertVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] lookups";

        Mediator.Enqueue(queue, title, command);
        return $"Successfully start new queue: {queue}";
    }

    [Authorize]// TODO: (Policy="Admin")
    [HttpPut($"{nameof(UpsertTimeline)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> UpsertTimeline([FromQuery] string licensePlate)
    {
        var command = new UpsertVehicleTimelineCommand(licensePlate);
        return await Mediator.Send(command);
    }

    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [Authorize]// TODO: (Policy="Admin")
    [HttpPut($"{nameof(UpsertTimelines)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertTimelines(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 1000
    )
    {
        var command = new UpsertVehicleTimelinesCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(UpsertVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] timelines";

        Mediator.Enqueue(queue, title, command);
        return $"Successfully start queue: {queue}";
    }

    [HttpPost($"{nameof(CreateServiceLog)}")]
    [ProducesResponseType(typeof(VehicleServiceLogDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogDtoItem> CreateServiceLog([FromForm] CreateVehicleServiceLogDto commandWithAttachment, CancellationToken cancellationToken)
    {
        var command = commandWithAttachment.ServiceLogCommand;

        // If a file is included, process it
        if (commandWithAttachment.AttachmentFile != null && commandWithAttachment.AttachmentFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await commandWithAttachment.AttachmentFile.CopyToAsync(memoryStream, cancellationToken);

            command.Attachment = new VehicleServiceLogAttachmentDtoItem
            {
                FileName = commandWithAttachment.AttachmentFile.FileName,
                FileData = memoryStream.ToArray()
            };
        }

        return await Mediator.Send(command, cancellationToken);
    }

    [HttpDelete($"{nameof(DeleteServiceLog)}/{{serviceLogId}}")]
    [Authorize]// TODO: (Policy="Admin") or garage owner matched with service log
    [ProducesResponseType(typeof(VehicleServiceLogDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogItem> DeleteServiceLog([FromRoute] Guid serviceLogId, CancellationToken cancellationToken)
    {
        var command = new DeleteVehicleServiceLogCommand(serviceLogId);
        return await Mediator.Send(command, cancellationToken);
    }

}
