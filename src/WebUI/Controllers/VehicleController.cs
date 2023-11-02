﻿using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleDefects;
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

    [HttpGet($"{nameof(GetMOTHistory)}")]
    [ProducesResponseType(typeof(VehicleDefectItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleDefectItem[]> GetMOTHistory([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleMOTHistoryQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecsDtoItem> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecsQuery(licensePlate));
    }


    //[HttpGet($"{nameof(GetCommonlyKnownIssues)}")]
    //[ProducesResponseType(typeof(VehicleSpecsDtoItem), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    //public async Task<VehicleSpecsDtoItem> GetCommonlyKnownIssues([FromQuery] string licensePlate)
    //{
    //    // TODO: call to get vehicle most commonly known issues
    //    throw new NotImplementedException();
    //}

    //[HttpPost($"{nameof(UpsertKnownDefects)}")]
    //[ProducesResponseType(typeof(VehicleLookupItem), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    //public async Task<VehicleLookupDtoItem> UpsertKnownDefects([FromBody] UpsertVehicleLookupCommand command)
    //{
    //    var response = await Mediator.Send(command);
    //    return response;
    //}


    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
    [Authorize]// TODO: (Policy="Admin")
    [HttpPut($"{nameof(UpsertLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertLookups(
        [FromQuery] int maxInsertAmount = 10, 
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] bool updateTimeline = true,
        [FromQuery] bool updateServiceLogs = true
    ) {
        var jobName = $"{nameof(UpsertVehicleLookupsCommand)}:maxInsert({maxInsertAmount}):maxUpdate({maxUpdateAmount})";
        var command = new UpsertVehicleLookupsCommand(maxInsertAmount, maxUpdateAmount, updateTimeline, updateServiceLogs);
        Mediator.Enqueue(jobName, command);

        return $"Successfully start hangfire job: {jobName}";
    }


}
