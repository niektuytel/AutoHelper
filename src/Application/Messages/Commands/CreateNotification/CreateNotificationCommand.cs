﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using AutoHelper.Application.Messages._DTOs;
using Hangfire;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using System.Text.Json.Serialization;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AutoHelper.Application.Messages.Commands.CreateNotificationMessage;

public record CreateNotificationCommand : IRequest<NotificationItem>
{

    public CreateNotificationCommand()
    {
        
    }

    public CreateNotificationCommand(
        string vehicleLicensePlate, 
        DateTime triggerDate,
        GeneralNotificationType generalType, 
        VehicleNotificationType vehicleType = VehicleNotificationType.Other,
        string? emailAddress = null,
        string? whatsappNumber = null,
        bool isRecurring = false
    )
    {
        VehicleLicensePlate = vehicleLicensePlate;
        TriggerDate = triggerDate;
        GeneralType = generalType;
        VehicleType = vehicleType;
        ReceiverEmailAddress = emailAddress;
        ReceiverWhatsappNumber = whatsappNumber;
        IsRecurring = isRecurring;
    }

    public bool IsRecurring { get; set; }

    public string VehicleLicensePlate { get; set; } = null!;

    public DateTime TriggerDate { get; set; }

    public GeneralNotificationType GeneralType { get; set; }

    public VehicleNotificationType VehicleType { get; set; }

    public string? ReceiverEmailAddress { get; set; } = null;

    public string? ReceiverWhatsappNumber { get; set; } = null;

    [JsonIgnore]
    public VehicleLookupItem? VehicleLookup { get; set; } = null!;

}

public class CreateNotificationMessageCommandHandler : IRequestHandler<CreateNotificationCommand, NotificationItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _identificationHelper;

    public CreateNotificationMessageCommandHandler(IApplicationDbContext context, IIdentificationHelper identificationHelper)
    {
        _context = context;
        _identificationHelper = identificationHelper;
    }

    public async Task<NotificationItem> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var receiverIdentifier = _identificationHelper.GetValidIdentifier(request.ReceiverEmailAddress, request.ReceiverWhatsappNumber);
        var receiverType = receiverIdentifier!.GetContactType();

        var notification = new NotificationItem
        {
            TriggerDate = request.TriggerDate,
            GeneralType = request.GeneralType,
            VehicleType = request.VehicleType,
            ReceiverContactType = receiverType,
            ReceiverContactIdentifier = receiverIdentifier, 
            VehicleLicensePlate = request.VehicleLicensePlate
        };

        // Only store recurring notifications
        if (request.IsRecurring)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return notification;
    }

}
