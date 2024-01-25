using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;
using AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.WebUI.Controllers;
using AutoHelper.Hangfire.Shared.MediatR;
using AutoMapper;
using MediatR;
using Hangfire;

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
    private readonly IBackgroundJobClient _backgroundJobClient;

    public DeleteVehicleServiceLogAsGarageCommandHandler(IVehicleService vehicleService, IApplicationDbContext context, IMapper mapper, ISender sender, IBackgroundJobClient backgroundJobClient)
    {
        _vehicleService = vehicleService;
        _context = context;
        _mapper = mapper;
        _sender = sender;
        _backgroundJobClient = backgroundJobClient;
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
        var notificationCommand = new CreateNotificationCommand(
            serviceLog.VehicleLicensePlate,
            GeneralNotificationType.VehicleServiceReviewDeclined,
            VehicleNotificationType.Other,
            triggerDate: null,
            emailAddress: serviceLog.ReporterEmailAddress,
            whatsappNumber: serviceLog.ReporterPhoneNumber
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        _sender.Enqueue(_backgroundJobClient, queue, title, schuduleCommand);
    }
}