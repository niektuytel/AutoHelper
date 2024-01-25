using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimeline;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using AutoHelper.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Models;
using YamlDotNet.Core.Tokens;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecifications;
using System;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Application.Messages.Commands.CreateConversationMessage;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Domain.Entities.Messages.Enums;
using Azure.Core;
using MediatR;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleEventNotifier;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;

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

    /// <param name="maxAmount">-1 means all of them</param>
    [HttpGet($"{nameof(GetTimeline)}")]
    [ProducesResponseType(typeof(VehicleTimelineDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleTimelineDtoItem[]> GetTimeline([FromQuery] string licensePlate, [FromQuery] int maxAmount = 5)
    {
        return await Mediator.Send(new GetVehicleTimelineQuery(licensePlate, maxAmount));
    }

    [HttpPost($"{nameof(CreateServiceLog)}")]
    [ProducesResponseType(typeof(VehicleServiceLogDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogDtoItem> CreateServiceLog([FromForm] CreateVehicleServiceLogDtoItem commandWithAttachment, CancellationToken cancellationToken)
    {
        // JsonIgnore does not work on the controller level, so we do the mapping
        var command = new CreateVehicleServiceLogCommand(commandWithAttachment);

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

    [HttpPost(nameof(CreateServiceEventNotifier))]
    [ProducesResponseType(typeof(NotificationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<NotificationItemDto> CreateServiceEventNotifier([FromBody] CreateVehicleEventNotifierCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpDelete($"{nameof(DeleteServiceEventNotifier)}/{{id}}")]
    [ProducesResponseType(typeof(NotificationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<NotificationItemDto> DeleteServiceEventNotifier([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new DeleteVehicleEventNotifierCommand(id), cancellationToken);
    }

}
