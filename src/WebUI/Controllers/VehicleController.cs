using System.Text;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecifications;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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

    [HttpGet($"vehicle/{nameof(ServicelogDeeplink)}")]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ServicelogDeeplink([FromQuery] string action)
    {
        // looks like: "{ "servicelogId": "b02192d5-a953-4e73-9867-b62bf98d4d38", "approve":true }"
        // the first part is the serviceLogId and the second is approve:1 or reject: 0
        var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(action));
        var parts = decoded.Split(':');
        var serviceLogId = parts[0];
        var approve = parts[1] == "1";

        return Redirect($"/vehicle#service_logs?approved={approve}");
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
    public async Task<VehicleTimelineDtoItem[]> GetTimeline([FromQuery] string licensePlate, [FromQuery] int maxAmount = 5)
    {
        return await Mediator.Send(new GetVehicleTimelineQuery(licensePlate, maxAmount));
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

    [HttpPost(nameof(CreateServiceEventNotifier))]
    [ProducesResponseType(typeof(NotificationItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<NotificationItemDto> CreateServiceEventNotifier([FromBody] CreateVehicleEventNotifierCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

}
