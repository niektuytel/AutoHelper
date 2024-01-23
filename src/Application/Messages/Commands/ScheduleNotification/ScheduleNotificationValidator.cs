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

namespace AutoHelper.Application.Messages.Commands.ScheduleNotification;

public class ScheduleNotificationValidator : AbstractValidator<ScheduleNotificationCommand>
{
    private readonly IApplicationDbContext _context;

    public ScheduleNotificationValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Notification)
            .NotEmpty()
            .WithMessage("Should have valid notification to schedule it");

    }

}
