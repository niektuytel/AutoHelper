using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleDefects;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
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

    [HttpGet($"{nameof(SearchByLicensePlate)}")]
    [ProducesResponseType(typeof(VehicleBriefDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleBriefDtoItem> SearchByLicensePlate([FromQuery] string licensePlate)
    {
        // TODO: add type vehicle for example car, truck, motorcycle or hatchback, sedan, suv
        return await Mediator.Send(new GetVehicleBriefInfoQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(VehicleServiceLogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogItemDto> GetServiceLogs([FromQuery] string licensePlate)
    {
        // TODO: call to get vehicle service logs >> GetVehicleServiceLogsQuery
        return await Mediator.Send(new GetVehicleServiceLogsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetDefects)}")]
    [ProducesResponseType(typeof(VehicleDefectItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleDefectItem[]> GetDefects([FromQuery] string licensePlate)
    {
        // TODO: call to get vehicle most commonly known issues
        return await Mediator.Send(new GetVehicleDefectsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecsDtoItem> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetCommonlyKnownIssues)}")]
    [ProducesResponseType(typeof(VehicleSpecsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecsDtoItem> GetCommonlyKnownIssues([FromQuery] string licensePlate)
    {
        // TODO: call to get vehicle most commonly known issues
        throw new NotImplementedException();
    }

    [HttpPost($"{nameof(UpsertLookup)}")]
    [ProducesResponseType(typeof(VehicleLookupItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleLookupDtoItem> UpsertLookup([FromBody] UpsertVehicleLookupCommand command)
    {
        var response = await Mediator.Send(command);
        return response;
    }


}
