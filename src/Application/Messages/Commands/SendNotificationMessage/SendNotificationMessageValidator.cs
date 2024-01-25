using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.SendNotificationMessage;

public class SendNotificationMessageValidator : AbstractValidator<SendNotificationMessageCommand>
{
    private readonly IApplicationDbContext _context;

    public SendNotificationMessageValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(BeValidAndExistingNotification)
            .WithMessage("Invalid or non-existent notification.");

    }

    private async Task<bool> BeValidAndExistingNotification(SendNotificationMessageCommand command, CancellationToken cancellationToken)
    {
        if (command.Notification == null && command.NotificationId != default)
        {
            var entity = _context.Notifications
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == command.NotificationId);

            command.Notification = entity;
        }

        return command.Notification != null;
    }

}
