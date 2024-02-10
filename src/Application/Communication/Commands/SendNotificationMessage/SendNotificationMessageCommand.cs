using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.DeleteNotification;
using AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;
using AutoHelper.Domain.Common.Enums;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Messages;
using MediatR;

namespace AutoHelper.Application.Messages.Commands.SendNotificationMessage;

public record SendNotificationMessageCommand : IQueueRequest<Unit>
{
    public SendNotificationMessageCommand(Guid notificationId)
    {
        NotificationId = notificationId;
    }

    public Guid NotificationId { get; set; }

    [JsonIgnore]
    public NotificationItem? Notification { get; set; } = null;

    [JsonIgnore]
    public IQueueContext QueueingService { get; set; } = null!;
}

public class SendNotificationMessageCommandHandler : IRequestHandler<SendNotificationMessageCommand, Unit>
{
    private readonly IWhatsappTemplateService _whatsappService;
    private readonly IMailingService _mailingService;
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly IQueueService _queueService;
    private readonly IVehicleService _vehicleService;

    public SendNotificationMessageCommandHandler(
        IWhatsappTemplateService whatsappService,
        IMailingService mailingService,
        IApplicationDbContext context,
        ISender sender,
        IQueueService queueService,
        IVehicleService vehicleService
    )
    {
        _whatsappService = whatsappService;
        _mailingService = mailingService;
        _context = context;
        _sender = sender;
        _queueService = queueService;
        _vehicleService = vehicleService;
    }

    public async Task<Unit> Handle(SendNotificationMessageCommand request, CancellationToken cancellationToken)
    {
        // send notification
        var receiverType = request.Notification!.ReceiverContactType;
        var receiverService = GetMessagingService(receiverType);

        var licensePlate = request.Notification.VehicleLicensePlate;
        var vehicle = await _vehicleService.GetTechnicalBriefByLicensePlateAsync(licensePlate);
        if (vehicle == null)
        {
            throw new InvalidDataException($"Vehicle not found: {licensePlate}");
        }

        await receiverService.SendNotificationMessage(request.Notification, vehicle, cancellationToken);

        // next notification
        _ = await HandleNextNotification(request.Notification, cancellationToken);

        return Unit.Value;
    }

    private IMessagingService GetMessagingService(ContactType contactType)
    {
        return contactType switch
        {
            ContactType.Email => _mailingService,
            ContactType.WhatsApp => _whatsappService,
            _ => throw new InvalidOperationException($"Invalid contact type: {contactType}"),
        };
    }

    /// <summary>
    /// Create a new notification, when it is a VehicleServiceNotification
    /// </summary>
    private async Task<NotificationItem?> HandleNextNotification(NotificationItem notification, CancellationToken cancellationToken)
    {
        // check if notification needs to continue
        if (notification.GeneralType != NotificationGeneralType.VehicleServiceNotification)
        {
            var deleteNotificationCommand = new DeleteNotificationCommand(notification.Id);
            _ = await _sender.Send(deleteNotificationCommand);

            return null;
        }

        // get next notifier
        var nextNotifierQuery = new GetVehicleNextNotificationQuery(notification.VehicleLicensePlate)
        {
            Vehicle = notification.RelatedVehicleLookup // avoid db call
        };
        var nextNotifier = await _sender.Send(nextNotifierQuery, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notification.VehicleLicensePlate}_{NotificationGeneralType.VehicleServiceNotification.ToString()}";
        var jobId = _queueService.ScheduleJob(queue, title, schuduleCommand, nextNotifier.TriggerDate);

        // update notification
        notification.TriggerDate = nextNotifier.TriggerDate;
        notification.VehicleType = nextNotifier.NotificationType;
        notification.JobId = jobId;

        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return notification;
    }



}
