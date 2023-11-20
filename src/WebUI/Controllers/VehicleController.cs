using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLog;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
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

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet($"{nameof(GetBriefInfo)}")]
    [ProducesResponseType(typeof(VehicleBriefDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleBriefDtoItem> GetBriefInfo([FromQuery] string licensePlate)
    {
        // TODO: add type vehicle for example car, truck, motorcycle or hatchback, sedan, suv
        // TODO: add total number of owners has owned this vehicle
        // TODO: advice on service of the car at given mileage
        return await Mediator.Send(new GetVehicleBriefInfoQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetTimeline)}")]
    [ProducesResponseType(typeof(VehicleServiceLogItemDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleTimelineDtoItem[]> GetTimeline([FromQuery] string licensePlate, [FromQuery] int maxAmount=5)
    {
        return await Mediator.Send(new GetVehicleTimelineQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(VehicleServiceLogItemDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogItemDto[]> GetServiceLogs([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleServiceLogsQuery(licensePlate));
    }

    //[HttpGet($"{nameof(GetMOTHistory)}")]
    //[ProducesResponseType(typeof(RDWDetectedDefect[]), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    //public async Task<RDWDetectedDefect[]> GetMOTHistory([FromQuery] string licensePlate)
    //{
    //    return await Mediator.Send(new GetVehicleMOTHistoryQuery(licensePlate));
    //}

    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecificationsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecificationsDto> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecsQuery(licensePlate));
    }

    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
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
        return $"Successfully start queue: {queue}";
    }

    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
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
    [ProducesResponseType(typeof(VehicleServiceLogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogItem> CreateServiceLog(
        [FromForm] CreateVehicleServiceLogWithAttachmentDto commandWithAttachment, 
        CancellationToken cancellationToken
    )
    {
        var command = commandWithAttachment.ServiceLogCommand;

        // If a file is included, process it
        if (commandWithAttachment.AttachmentFile != null && commandWithAttachment.AttachmentFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await commandWithAttachment.AttachmentFile.CopyToAsync(memoryStream, cancellationToken);

            command.Attachment = new VehicleServiceLogAttachmentItemOnCreateDto
            {
                FileName = commandWithAttachment.AttachmentFile.FileName,
                FileData = memoryStream.ToArray()
            };
        }

        return await Mediator.Send(command, cancellationToken);
    }

    [HttpDelete($"{nameof(DeleteServiceLog)}/{{serviceLogId}}")]
    [Authorize]// TODO: (Policy="Admin")
    [ProducesResponseType(typeof(VehicleServiceLogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogItem> DeleteServiceLog([FromRoute] Guid serviceLogId, CancellationToken cancellationToken)
    {
        var command = new DeleteVehicleServiceLogCommand(serviceLogId);
        return await Mediator.Send(command, cancellationToken);
    }
}
