using System.Security.Claims;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.DeleteGarageService;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Commands.UpdateGarageService;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLogAsGarage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.UpdateVehicleServiceLogAsGarage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogsAsGarage;
using System.Threading;
using AutoHelper.Application.Garages.Queries.GetGarageStatistics;

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
    [ProducesResponseType(typeof(GarageSettingsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageSettingsDtoItem> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageSettingsQuery(userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetServices)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceDtoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceDtoItem>> GetServices([FromQuery] string? licensePlate = null)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageServicesQuery(userId, licensePlate));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleServiceLogAsGarageDtoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<VehicleServiceLogAsGarageDtoItem>> GetServiceLogs([FromQuery] string? licensePlate=null)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetVehicleServiceLogsAsGarageQuery(userId, licensePlate));
    }

    [Authorize]
    [HttpPost($"{nameof(CreateGarage)}")]
    [ProducesResponseType(typeof(GarageSettingsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GarageSettingsDtoItem>> CreateGarage([FromBody] CreateGarageCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        var userName = _currentUser.UserName ?? command.GarageLookupIdentifier;
        var userEmail = _currentUser.UserEmail ?? command.EmailAddress;
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
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPost($"{nameof(CreateServiceLog)}")]
    [ProducesResponseType(typeof(VehicleServiceLogAsGarageDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogAsGarageDtoItem> CreateServiceLog([FromForm] CreateVehicleServiceAsGarageLogDtoItem serviceLogDto, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");

        // JsonIgnore does not work on the controller level, so we do the mapping
        var command = new CreateVehicleServiceLogAsGarageCommand(userId, serviceLogDto);

        // If a file is included, process it
        if (serviceLogDto.AttachmentFile != null && serviceLogDto.AttachmentFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await serviceLogDto.AttachmentFile.CopyToAsync(memoryStream, cancellationToken);

            command.Attachment = new VehicleServiceLogAttachmentDtoItem
            {
                FileName = serviceLogDto.AttachmentFile.FileName,
                FileData = memoryStream.ToArray()
            };
        }

        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateSettings)}")]
    [ProducesResponseType(typeof(GarageSettingsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageSettingsDtoItem> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(UpdateService)}")]
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPost($"{nameof(UpdateServiceLog)}")]
    [ProducesResponseType(typeof(VehicleServiceLogAsGarageDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogAsGarageDtoItem> UpdateServiceLog([FromForm] UpdateVehicleServiceAsGarageLogDtoItem serviceLogDto, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");

        // JsonIgnore does not work on the controller level, so we do the mapping
        var command = new UpdateVehicleServiceLogAsGarageCommand(userId, serviceLogDto);

        // If a file is included, process it
        if (serviceLogDto.AttachmentFile != null && serviceLogDto.AttachmentFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await serviceLogDto.AttachmentFile.CopyToAsync(memoryStream, cancellationToken);

            command.Attachment = new VehicleServiceLogAttachmentDtoItem
            {
                FileName = serviceLogDto.AttachmentFile.FileName,
                FileData = memoryStream.ToArray()
            };
        }

        return await Mediator.Send(command);
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(DeleteService)}/{{id}}")]
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> DeleteService([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageServiceCommand(id, userId));
    }

    [Authorize(Policy = "GarageRole")]
    [HttpPut($"{nameof(DeleteServiceLog)}/{{id}}")]
    [ProducesResponseType(typeof(VehicleServiceLogAsGarageDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogAsGarageDtoItem> DeleteServiceLog([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteVehicleServiceLogAsGarageCommand(userId, id));
    }

}
