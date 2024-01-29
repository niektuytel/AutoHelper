using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Messages.Commands.CreateNotificationMessage;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoHelper.WebUI.Controllers;
using Hangfire;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Hangfire.Shared.MediatR;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public record CreateVehicleEventNotifierCommand : IRequest<NotificationItemDto>
{
    public string VehicleLicensePlate { get; set; } = null!;

    public string? ContactIdentifier { get; set; } = null;

    [JsonIgnore]
    public VehicleLookupItem? VehicleLookup { get; set; } = null!;

}

public class CreateVehicleEventNotifierCommandHandler : IRequestHandler<CreateVehicleEventNotifierCommand, NotificationItemDto>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public CreateVehicleEventNotifierCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IBackgroundJobClient backgroundJobClient, IMapper mapper, ISender sender)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _backgroundJobClient = backgroundJobClient;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<NotificationItemDto> Handle(CreateVehicleEventNotifierCommand request, CancellationToken cancellationToken)
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
            GeneralNotificationType.VehicleServiceNotification,
            nextNotifier.NotificationType,
            nextNotifier.TriggerDate,
            request.ContactIdentifier
        );
        var notification = await _sender.Send(notificationCommand, cancellationToken);

        // schedule notification
        var queue = nameof(SendNotificationMessageCommand);
        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var title = $"{notificationCommand.VehicleLicensePlate}_{notification.GeneralType.ToString()}";
        var jobId = _sender.ScheduleJob(_backgroundJobClient, queue, title, schuduleCommand, nextNotifier.TriggerDate);

        // update notification with job id
        notification.JobId = jobId;
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(notification);
    }

}
