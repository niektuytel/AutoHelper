using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.ReviewVehicleServiceLog;

public record ReviewVehicleServiceLogCommand : IRequest<VehicleServiceLogDtoItem>
{
    public ReviewVehicleServiceLogCommand()
    {

    }

    public ReviewVehicleServiceLogCommand(string actionString)
    {
        ActionString = actionString;
    }

    public ReviewVehicleServiceLogCommand(VehicleServiceLogItem serviceLog, bool approved)
    {
        ServiceLog = serviceLog;
        Approved = approved;
    }

    internal string? ActionString { get; set; } = null;
    internal bool Approved { get; set; } = false;
    internal VehicleServiceLogItem ServiceLog { get; set; } = null!;
}

public class UpdateVehicleServiceLogAsGarageCommandHandler : IRequestHandler<ReviewVehicleServiceLogCommand, VehicleServiceLogDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    private readonly IQueueService _queueService;
    private readonly IIdentificationHelper _identificationHelper;

    public UpdateVehicleServiceLogAsGarageCommandHandler(
        IApplicationDbContext context,
        ISender sender,
        IMapper mapper,
        IQueueService queueService,
        IIdentificationHelper identificationHelper
    )
    {
        _context = context;
        _sender = sender;
        _mapper = mapper;
        _queueService = queueService;
        _identificationHelper = identificationHelper;
    }

    public async Task<VehicleServiceLogDtoItem> Handle(ReviewVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        if (request.Approved)
        {
            return await HandleApprovedServiceLog(request, cancellationToken);
        }

        return await HandleDeclinedServiceLog(request, cancellationToken);
    }

    private async Task<VehicleServiceLogDtoItem> HandleApprovedServiceLog(ReviewVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        // already been verified
        if (request.ServiceLog.Status == VehicleServiceLogStatus.VerifiedByGarage)
        {
            return _mapper.Map<VehicleServiceLogDtoItem>(request.ServiceLog);
        }

        request.ServiceLog.Status = VehicleServiceLogStatus.VerifiedByGarage;
        _context.VehicleServiceLogs.Update(request.ServiceLog);
        await _context.SaveChangesAsync(cancellationToken);

        // insert timeline
        var timelineCommand = new CreateVehicleTimelineCommand(request.ServiceLog);
        await _sender.Send(timelineCommand, cancellationToken);

        await SendNotification(request.ServiceLog, NotificationGeneralType.VehicleServiceReviewApproved, cancellationToken);

        return _mapper.Map<VehicleServiceLogDtoItem>(request.ServiceLog);
    }

    private async Task<VehicleServiceLogDtoItem> HandleDeclinedServiceLog(ReviewVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        // deleting timeline
        var deleteTimelineCommand = new DeleteVehicleTimelineCommand(request.ServiceLog.Id);
        var deletedTimeline = await _sender.Send(deleteTimelineCommand, cancellationToken);

        _context.VehicleServiceLogs.Remove(request.ServiceLog);
        await _context.SaveChangesAsync(cancellationToken);

        await SendNotification(
            request.ServiceLog,
            NotificationGeneralType.VehicleServiceReviewDeclined,
            cancellationToken
        );

        return _mapper.Map<VehicleServiceLogDtoItem>(request.ServiceLog);
    }

    private async Task SendNotification(VehicleServiceLogItem serviceLog, NotificationGeneralType notificationType, CancellationToken cancellationToken)
    {
        var contactIdentifier = _identificationHelper.GetValidIdentifier(serviceLog.ReporterEmailAddress, serviceLog.ReporterPhoneNumber);
        var notificationCommand = new CreateNotificationCommand(
            serviceLog.VehicleLicensePlate,
            notificationType,
            NotificationVehicleType.Other,
            triggerDate: null,
            contactIdentifier: contactIdentifier
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        var queue = nameof(SendNotificationMessageCommand);
        var scheduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notificationType}";
        _queueService.Enqueue(queue, title, scheduleCommand);
    }

}
