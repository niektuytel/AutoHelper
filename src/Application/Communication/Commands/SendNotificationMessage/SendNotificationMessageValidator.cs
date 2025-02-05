﻿using AutoHelper.Application.Common.Interfaces;

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
        if (command.Notification != null)
        {
            command.NotificationId = command.Notification.Id;
        }
        else if (command.NotificationId != default)
        {
            var entity = _context.Notifications
                .AsNoTracking()
                .Include(x => x.RelatedVehicleLookup)
                .FirstOrDefault(x => x.Id == command.NotificationId);

            command.Notification = entity;
        }


        return command.Notification != null;
    }

}
