using System.Security.Claims;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Commands.DeleteGarageEmployee;
using AutoHelper.Application.Garages.Commands.DeleteGarageService;
using AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Commands.UpdateGarageService;
using AutoHelper.Application.Garages.Queries.GetGarageEmployees;
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.Vehicles.Queries;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities.Garages;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models.Response;
using AutoHelper.Application.Garages.Queries.GetGarageLookup;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages.Queries.GetGarageServiceTypesByLicensePlate;

namespace AutoHelper.WebUI.Controllers;

[Authorize]// TODO: (Policy="Admin")
public class GarageSyncController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GarageSyncController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }


    [HttpGet($"{nameof(AnalyzeGarageLookupData)}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<int> AnalyzeGarageLookupData(CancellationToken cancellationToken)
    {
        // TODO: Check if all current garages are up to date
        return -1;
    }

    [HttpPut($"{nameof(UpsertGarageLookups)}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<bool> UpsertGarageLookups(CancellationToken cancellationToken, [FromQuery] int maxInsertAmount = 1000)
    {
        // TODO: Start here hangfire sync tool

        return false;
    }
}
