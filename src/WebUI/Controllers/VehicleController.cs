using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.DeleteNotification;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecifications;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using Microsoft.AspNetCore.Mvc;

namespace AutoHelper.WebUI.Controllers;

/// <summary>
/// Manages vehicle-related operations such as retrieving vehicle specifications, service logs, and timelines, as well as updating vehicle data.
/// </summary>
public class VehicleController : ApiControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet($"{nameof(GetSpecificationsCard)}")]
    [ProducesResponseType(typeof(VehicleSpecificationsCardItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecificationsCardItem> GetSpecificationsCard([FromQuery] string licensePlate)
    {
        // TODO: advice on service of the car at given mileage
        return await Mediator.Send(new GetVehicleSpecificationsCardQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetSpecifications)}")]
    [ProducesResponseType(typeof(VehicleSpecificationsDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<VehicleSpecificationsDtoItem> GetSpecifications([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleSpecificationsQuery(licensePlate));
    }

    [HttpGet($"{nameof(GetServiceLogs)}")]
    [ProducesResponseType(typeof(VehicleServiceLogCardDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogCardDtoItem[]> GetServiceLogs([FromQuery] string licensePlate)
    {
        return await Mediator.Send(new GetVehicleServiceLogsQuery(licensePlate));
    }

    /// <param name="maxAmount">-1 means all of them</param>
    [HttpGet($"{nameof(GetTimeline)}")]
    [ProducesResponseType(typeof(VehicleTimelineDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<VehicleTimelineDtoItem[]> GetTimeline([FromQuery] string licensePlate, CancellationToken cancellationToken, [FromQuery] int maxAmount = 5)
    {
        var vehicleTimelineQuery = new GetVehicleTimelineQuery(licensePlate, maxAmount);
        var vehicleTimeline = await Mediator.Send(vehicleTimelineQuery, cancellationToken);

        return vehicleTimeline;
    }

    [HttpGet($"{nameof(UnsubscribeNotification)}/{{notificationId}}")]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UnsubscribeNotification([FromRoute] Guid notificationId, CancellationToken cancellationToken)
    {
        var deleteNotificationCommand = new DeleteNotificationCommand(notificationId);
        var deleteNotification =  await Mediator.Send(deleteNotificationCommand, cancellationToken);

        return Redirect($"/thankyou/{nameof(UnsubscribeNotification)}");
    }

    [HttpPost($"{nameof(CreateServiceLog)}")]
    [ProducesResponseType(typeof(VehicleServiceLogDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<VehicleServiceLogDtoItem> CreateServiceLog([FromForm] CreateVehicleServiceLogDtoItem commandWithAttachment, CancellationToken cancellationToken)
    {
        // JsonIgnore does not work on the controller level, so we do the mapping
        var command = new CreateVehicleServiceLogCommand(commandWithAttachment);

        // If a file is included, process it
        if (commandWithAttachment.AttachmentFile != null && commandWithAttachment.AttachmentFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await commandWithAttachment.AttachmentFile.CopyToAsync(memoryStream, cancellationToken);

            command.Attachment = new VehicleServiceLogAttachmentDtoItem
            {
                FileName = commandWithAttachment.AttachmentFile.FileName,
                FileData = memoryStream.ToArray()
            };
        }

        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPost(nameof(CreateNotification))]
    [ProducesResponseType(typeof(NotificationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<NotificationItemDto> CreateNotification([FromBody] CreateVehicleNotificationCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

}
