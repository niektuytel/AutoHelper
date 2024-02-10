using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLogAsGarage;

public class DeleteVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public DeleteVehicleServiceLogAsGarageCommand(string userId, Guid serviceLogId)
    {
        UserId = userId;
        ServiceLogId = serviceLogId;
    }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public GarageItem Garage { get; set; } = null!;

    public Guid ServiceLogId { get; set; }

    [JsonIgnore]
    public VehicleServiceLogItem ServiceLog { get; set; } = null!;

}

public class DeleteVehicleServiceLogAsGarageCommandHandler : IRequestHandler<DeleteVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IVehicleService _vehicleService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IQueueService _queueService;
    private readonly IIdentificationHelper _identificationHelper;

    public DeleteVehicleServiceLogAsGarageCommandHandler(
        IVehicleService vehicleService,
        IApplicationDbContext context,
        IMapper mapper,
        ISender sender,
        IQueueService queueService,
        IIdentificationHelper identificationHelper
    )
    {
        _vehicleService = vehicleService;
        _context = context;
        _mapper = mapper;
        _sender = sender;
        _queueService = queueService;
        _identificationHelper = identificationHelper;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(DeleteVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var timelineItem = await _sender.Send(new DeleteVehicleTimelineCommand(request.ServiceLogId), cancellationToken);

        _context.VehicleServiceLogs.Remove(request.ServiceLog);
        await _context.SaveChangesAsync(cancellationToken);
        //entity.AddDomainEvent(new SomeDomainEvent(entity));

        await SendNotificationToReporter(request.ServiceLog, cancellationToken);

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(request.ServiceLog);
    }

    private async Task SendNotificationToReporter(VehicleServiceLogItem serviceLog, CancellationToken cancellationToken)
    {
        var contactIdentifier = _identificationHelper.GetValidIdentifier(serviceLog.ReporterEmailAddress, serviceLog.ReporterPhoneNumber);
        var notificationCommand = new CreateNotificationCommand(
            serviceLog.VehicleLicensePlate,
            NotificationGeneralType.VehicleServiceReviewDeclined,
            NotificationVehicleType.Other,
            triggerDate: null,
            contactIdentifier: contactIdentifier
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        _queueService.Enqueue(queue, title, schuduleCommand);
    }
}