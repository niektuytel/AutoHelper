using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
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
        return await Mediator.Send(new GetVehicleBriefInfoQuery(licensePlate));
    }
    
    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecsDtoItem> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecsQuery(licensePlate));
    }

    [HttpPost($"{nameof(CreateLookup)}")]
    [ProducesResponseType(typeof(VehicleLookupItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleLookupDtoItem> CreateLookup([FromBody] UpsertVehicleLookupCommand command)
    {
        var response = await Mediator.Send(command);
        return response;
    }

}
