using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.WebUI.Controllers;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public record CreateVehicleNotificationCommand : IRequest<NotificationItemDto>
{
    public string VehicleLicensePlate { get; set; } = null!;

    public string? ContactIdentifier { get; set; } = null;

    [JsonIgnore]
    public VehicleLookupItem? VehicleLookup { get; set; } = null!;

}

public class CreateVehicleNotificationCommandHandler : IRequestHandler<CreateVehicleNotificationCommand, NotificationItemDto>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IQueueService _queueService;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public CreateVehicleNotificationCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IQueueService queueService, IMapper mapper, ISender sender)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _queueService = queueService;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<NotificationItemDto> Handle(CreateVehicleNotificationCommand request, CancellationToken cancellationToken)
    {
        // get next notifier, and avoid db call to parse instance
        var nextNotifierQuery = new GetVehicleNextNotificationQuery(request.VehicleLicensePlate)
        {
            Vehicle = request.VehicleLookup
        };
        var nextNotifier = await _sender.Send(nextNotifierQuery, cancellationToken);

        // create notification
        var notificationCommand = new CreateNotificationCommand(
            request.VehicleLicensePlate,
            NotificationGeneralType.VehicleServiceNotification,
            nextNotifier.NotificationType,
            nextNotifier.TriggerDate,
            request.ContactIdentifier
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        var jobId = _queueService.ScheduleJob(queue, title, schuduleCommand, nextNotifier.TriggerDate);

        // update notification with job id
        notification.JobId = jobId;
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(notification);
    }

}
