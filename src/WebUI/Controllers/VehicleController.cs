using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
using WebUI.Models.Response;
using YamlDotNet.Core.Tokens;

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{
    private readonly IVehicleInfoService _vehicleService;

    public VehicleController(IVehicleInfoService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchVehicle([FromQuery] string licensePlate)
    {
        try
        {
            if(string.IsNullOrEmpty(licensePlate))
            {
                return BadRequest("License plate is required");
            }

            licensePlate = licensePlate.Replace("-", "").ToUpper();

            var isValid = await _vehicleService.ValidVehicle(licensePlate);
            if (!isValid)
            {
                return BadRequest($"Invalid license plate: {licensePlate}");
            }

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
    
    [HttpGet($"{nameof(GetVehicleBriefInfo)}")]
    [ProducesResponseType(typeof(VehicleBriefInfoItemDto), StatusCodes.Status200OK)]
    public async Task<VehicleBriefInfoItemDto> GetVehicleBriefInfo([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleBriefInfoQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetVehicleInfo)}")]
    [ProducesResponseType(typeof(VehicleInfoItemDto), StatusCodes.Status200OK)]
    public async Task<VehicleInfoItemDto> GetVehicleInfo([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleInfoQuery(licensePlate));
    }

    [HttpPost($"{nameof(CreateVehicleLookup)}")]
    [ProducesResponseType(typeof(VehicleLookupItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleLookupDtoItem> CreateVehicleLookup([FromBody] CreateVehicleLookupCommand command)
    {
        var response = await Mediator.Send(command);
        return response;
    }

}
