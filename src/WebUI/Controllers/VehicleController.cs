using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
using WebUI.Models.Response;
using YamlDotNet.Core.Tokens;

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<LicencePlateBriefResponse>> SearchVehicle([FromQuery] string licensePlate)
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
}
