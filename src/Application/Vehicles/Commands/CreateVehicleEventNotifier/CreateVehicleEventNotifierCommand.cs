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

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public record CreateVehicleEventNotifierCommand : IRequest<NotificationItemDto>
{
    public string VehicleLicensePlate { get; set; } = null!;

    public string? ReceiverEmailAddress { get; set; } = null;

    public string? ReceiverWhatsappNumber { get; set; } = null;

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
        var command = new CreateNotificationCommand(
            request.VehicleLicensePlate,
            NotificationType.VehicleServiceNotification,
            request.ReceiverEmailAddress,
            request.ReceiverWhatsappNumber,
            true
        );
        var notification = await _sender.Send(command, cancellationToken);

        // TODO: reconfigure this + calculate the next date
        var title = $"{reque TODO st.VehicleLicensePlate}_{NotificationType.VehicleServiceNotification.ToString()}";
        var dateTime = DateTime.Now.AddMinutes(2);

        var schuduleCommand = new SendNotificationMessageCommand(notification.Id);
        var queue = nameof(SendNotificationMessageCommand);

        var jobId = _sender.ScheduleJob(_backgroundJobClient, queue, title, schuduleCommand, dateTime);
        notification.JobId = jobId;

        // update notification with job id
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(notification);
    }
}
