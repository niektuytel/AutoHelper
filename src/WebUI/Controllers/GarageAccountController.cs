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
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using Microsoft.Identity.Web.Resource;
using Microsoft.Identity.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoHelper.WebUI.Controllers;

public class GarageAccountController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public GarageAccountController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [Authorize(Policy = "UserReadWritePolicy")]
    [HttpGet($"{nameof(GetSettings)}")]
    [ProducesResponseType(typeof(GarageSettingsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageSettingsDtoItem> GetSettings()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageSettingsQuery(userId.ToString()));
    }

    [HttpGet($"{nameof(GetServices)}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceDtoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceDtoItem>> GetServices([FromQuery] string? licensePlate = null)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageServicesQuery(userId, licensePlate));
    }

    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(IEnumerable<VehicleServiceLogAsGarageDtoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<VehicleServiceLogAsGarageDtoItem>> GetServiceLogs([FromQuery] string? licensePlate=null)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetVehicleServiceLogsAsGarageQuery(userId, licensePlate));
    }

    [Authorize(Policy = "UserReadWritePolicy")]
    [HttpGet($"{nameof(GetOverview)}")]
    [ProducesResponseType(typeof(GarageOverviewDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageOverviewDtoItem> GetOverview()
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new GetGarageOverviewQuery(userId));
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
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateService)}")]
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

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

    [HttpPut($"{nameof(UpdateSettings)}")]
    [ProducesResponseType(typeof(GarageSettingsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageSettingsDtoItem> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateService)}")]
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        command.UserId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(command);
    }

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

    [HttpPut($"{nameof(DeleteService)}/{{id}}")]
    [ProducesResponseType(typeof(GarageServiceDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageServiceDtoItem> DeleteService([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteGarageServiceCommand(id, userId));
    }

    [HttpPut($"{nameof(DeleteServiceLog)}/{{id}}")]
    [ProducesResponseType(typeof(VehicleServiceLogAsGarageDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogAsGarageDtoItem> DeleteServiceLog([FromRoute] Guid id)
    {
        var userId = _currentUser.UserId ?? throw new Exception("Missing userId on IdToken");
        return await Mediator.Send(new DeleteVehicleServiceLogAsGarageCommand(userId, id));
    }

}
