using System;
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

namespace AutoHelper.Application.Messages.Commands.ScheduleNotification;

public record ScheduleNotificationCommand : IRequest<NotificationItem>
{

    public ScheduleNotificationCommand(NotificationItem notification)
    {
        Notification = notification;
    }

    public NotificationItem Notification { get; set; }

}

public class CreateNotificationMessageCommandHandler : IRequestHandler<ScheduleNotificationCommand, NotificationItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _identificationHelper;

    public CreateNotificationMessageCommandHandler(IApplicationDbContext context, IIdentificationHelper identificationHelper)
    {
        _context = context;
        _identificationHelper = identificationHelper;
    }

    public async Task<NotificationItem> Handle(ScheduleNotificationCommand request, CancellationToken cancellationToken)
    {
        var receiverIdentifier = _identificationHelper.GetValidIdentifier(request.ReceiverEmailAddress, request.ReceiverWhatsappNumber);
        var receiverType = receiverIdentifier!.GetContactType();

        var notification = new NotificationItem
        {
            Cron = request.Cron,
            Type = request.Type,
            ReceiverContactType = receiverType,
            ReceiverContactIdentifier = receiverIdentifier,
            VehicleLicensePlate = request.VehicleLicensePlate
        };

        // Only store to the database if the cron is not null
        if (!string.IsNullOrEmpty(request.Cron))
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return notification;
    }

}
