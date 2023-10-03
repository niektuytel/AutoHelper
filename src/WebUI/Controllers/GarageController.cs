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
    [ProducesResponseType(typeof(GarageItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageItemDto> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageSettingsQuery(userId));
    }

    [HttpGet($"{nameof(GetOverview)}")]
    [ProducesResponseType(typeof(GarageOverview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageOverview> GetOverview()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageOverviewQuery(userId));
    }

    [HttpGet($"{nameof(GetServices)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceItemDto>> GetServices()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageServicesQuery(userId));
    }

    [HttpGet($"{nameof(GetEmployees)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageEmployeeItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageEmployeeItemDto>> GetEmployees()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageEmployeesQuery(userId));
    }

    [HttpPost($"{nameof(CreateService)}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateEmployee)}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> CreateEmployee([FromBody] CreateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateSettings)}")]
    [ProducesResponseType(typeof(GarageItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageItem> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateService)}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateEmployee)}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> UpdateEmployee([FromBody] UpdateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(DeleteService)}/{{id}}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> DeleteService([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageServiceCommand(id, userId));
    }

    [HttpPut($"{nameof(DeleteEmployee)}/{{id}}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> DeleteEmployee([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageEmployeeCommand(id, userId));
    }

}
