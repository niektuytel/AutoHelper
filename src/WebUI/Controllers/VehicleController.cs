﻿using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
using WebUI.Models.Response;
using WebUI.Services;
using YamlDotNet.Core.Tokens;

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{

    private readonly IVehicleInformationService _vehicleInformationService;

    public VehicleController(IVehicleInformationService vehicleInformationService)
    {
        _vehicleInformationService = vehicleInformationService;
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

            var isValid = await _vehicleInformationService.ValidVehicle(licensePlate);
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

    [HttpGet("information")]
    public async Task<ActionResult<VehicleInformationResponse>> GetVehicleInformation([FromQuery] string licensePlate)
    {
        try
        {
            licensePlate = licensePlate.Replace("-", "").ToUpper();

            var result = await _vehicleInformationService.GetVehicleInformationAsync(licensePlate);
            return Ok(result);
        }
        catch (NotFoundException nfe)
        {
            return NotFound(nfe.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }


}
