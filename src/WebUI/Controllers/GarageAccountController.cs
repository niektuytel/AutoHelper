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
using AutoHelper.Domain.Entities.Garages;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

namespace AutoHelper.WebUI.Controllers;

public class GarageAccountController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;

    public GarageAccountController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetSettings)}")]
    [ProducesResponseType(typeof(GarageItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageItemDto> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageSettingsQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetOverview)}")]
    [ProducesResponseType(typeof(GarageOverview), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageOverview> GetOverview()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageOverviewQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetServices)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceItemDto>> GetServices()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageServicesQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetEmployees)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageEmployeeItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageEmployeeItemDto>> GetEmployees()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageEmployeesQuery(userId));
    }

    [Authorize]
    [HttpPost($"{nameof(Create)}")]
    [ProducesResponseType(typeof(GarageItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GarageItem>> Create([FromBody] CreateGarageCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        var userName = _currentUser.UserName ?? command.Name;
        var userEmail = _currentUser.UserEmail ?? command.Email;
        var result = await Mediator.Send(command);
        if (result != null)
        {
            // Register user with an garage Role
            await _identityService.SetUserWithRoleAsync(command.UserId, userName, userEmail, "Garage");

            return result;
        }

        return BadRequest("Could not create garage");
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPost($"{nameof(CreateService)}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPost($"{nameof(CreateEmployee)}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> CreateEmployee([FromBody] CreateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateSettings)}")]
    [ProducesResponseType(typeof(GarageItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageItem> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateService)}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateEmployee)}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> UpdateEmployee([FromBody] UpdateGarageEmployeeCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(DeleteService)}/{{id}}")]
    [ProducesResponseType(typeof(GarageServiceItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceItem> DeleteService([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageServiceCommand(id, userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(DeleteEmployee)}/{{id}}")]
    [ProducesResponseType(typeof(GarageEmployeeItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageEmployeeItem> DeleteEmployee([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageEmployeeCommand(id, userId));
    }

}
