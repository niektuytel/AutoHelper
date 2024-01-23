
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
using AutoHelper.Application.Messages.Commands.ScheduleNotification;
using AutoHelper.WebUI.Controllers;
using AutoHelper.Domain.Entities.Messages;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleEventNotifier;

public record DeleteVehicleEventNotifierCommand : IRequest<NotificationItemDto>
{
    public string Identifier { get; set; } = null!;

    public string VehicleLicensePlate { get; set; } = null!;

    [JsonIgnore]
    public NotificationItem? Notification { get; set; } = null!;

}

public class DeleteVehicleEventNotifierCommandHandler : IRequestHandler<DeleteVehicleEventNotifierCommand, NotificationItemDto>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMediator _sender;

    public DeleteVehicleEventNotifierCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IMapper mapper, IMediator sender)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<NotificationItemDto> Handle(DeleteVehicleEventNotifierCommand request, CancellationToken cancellationToken)
    {
        // TODO: STOP THE SCHEDULED NOTIFICATION
        var schuduleCommand = new ScheduleNotificationCommand(notification);
        _sender.Schedule(() => _sender.Send(schuduleCommand, cancellationToken), notification.Cron);

        var result = _context.Notifications.Remove(request.Notification!);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<NotificationItemDto>(result);
    }
}
