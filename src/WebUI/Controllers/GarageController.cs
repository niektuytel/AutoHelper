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

[Authorize(Policy = "GarageRole")]
public class GarageController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GarageController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    [HttpGet($"{nameof(GetSettings)}")]
    public async Task<GarageItemDto> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageSettingsQuery(userId));
    }

    [HttpGet($"{nameof(GetOverview)}")]
    public async Task<GarageOverview> GetOverview()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageOverviewQuery(userId));
    }

    [HttpGet($"{nameof(GetServices)}")]
    public async Task<IEnumerable<GarageServiceItemDto>> GetServices()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageServicesQuery(userId));
    }

    [HttpGet($"{nameof(GetEmployees)}")]
    public async Task<IEnumerable<GarageEmployeeItemDto>> GetEmployees()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageEmployeesQuery(userId));
    }

    [HttpPost($"{nameof(CreateService)}")]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateEmployee)}")]
    public async Task<GarageEmployeeItem> CreateEmployee([FromBody] CreateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateSettings)}")]
    public async Task<GarageItem> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateService)}")]
    public async Task<GarageServiceItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateEmployee)}")]
    public async Task<GarageEmployeeItem> UpdateEmployee([FromBody] UpdateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(DeleteService)}/{{id}}")]
    public async Task<GarageServiceItem> DeleteService([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageServiceCommand(id, userId));
    }

    [HttpPut($"{nameof(DeleteEmployee)}/{{id}}")]
    public async Task<GarageEmployeeItem> DeleteEmployee([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageEmployeeCommand(id, userId));
    }

}
