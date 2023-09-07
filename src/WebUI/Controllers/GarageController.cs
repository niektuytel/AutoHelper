using System.Security.Claims;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Commands.UpdateGarageService;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoHelper.WebUI.Controllers;

public class GarageController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GarageController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetOverview)}")]
    public async Task<GarageOverview> GetOverview()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(new GetGarageOverviewQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetServices)}")]
    public async Task<IEnumerable<GarageServiceItem>> GetServices()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(new GetGarageServicesQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetSettings)}")]
    public async Task<GarageSettings> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(new GetGarageSettingsQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateSettings)}")]
    public async Task<GarageSettings> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateService)}")]
    public async Task<GarageServiceItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(command);
    }

    [Authorize]
    [HttpPost($"{nameof(CreateGarage)}")]
    public async Task<ActionResult<GarageSettings>> CreateGarage([FromBody] CreateGarageCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId");
        var result = await Mediator.Send(command);
        if (result != null)
        {
            // Register user with an garage Role
            await _identityService.SetUserWithRoleAsync(command.UserId, command.Name, command.Email, "Garage");

            return result;
        }

        return BadRequest("Could not create garage");
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPost($"{nameof(CreateService)}")]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId");
        return await Mediator.Send(command);
    }

}
