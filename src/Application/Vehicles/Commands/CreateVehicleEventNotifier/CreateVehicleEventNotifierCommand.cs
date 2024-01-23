﻿using System.ComponentModel.DataAnnotations;
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

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public record CreateVehicleEventNotifierCommand : IRequest<NotificationItemDto>
{
    public string VehicleLicensePlate { get; set; } = null!;

    public string? ReceiverEmailAddress { get; set; } = null;

    public string? ReceiverWhatsappNumber { get; set; } = null;

    [JsonIgnore]
    public string Cron { get; set; } = "0 3 * * 1";// Every Monday at 3:00 AM

    [JsonIgnore]
    public VehicleLookupItem? VehicleLookup { get; set; } = null!;

}

public class CreateVehicleEventNotifierCommandHandler : IRequestHandler<CreateVehicleEventNotifierCommand, NotificationItemDto>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMediator _sender;

    public CreateVehicleEventNotifierCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IMapper mapper, IMediator sender)
    {
        _blobStorageService = blobStorageService;
        _context = context;
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
            request.Cron
        );
        var notification = await _sender.Send(command, cancellationToken);

        // Schedule the notification
        var schuduleCommand = new ScheduleNotificationCommand(notification);
        _sender.Schedule(() => _sender.Send(schuduleCommand, cancellationToken), notification.Cron);


        return _mapper.Map<NotificationItemDto>(notification);
    }
}
