using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Application.Conversations.Commands.SendConversationMessage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.CreateNotificationMessage;

public class CreateNotificationValidator : AbstractValidator<CreateNotificationCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateNotificationValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x)
            .Must(HaveEmailOrWhatsapp)
            .WithMessage("Either an email address or a WhatsApp number must be provided.");
    }

    private async Task<bool> BeValidAndExistingVehicle(CreateNotificationCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");

        command.VehicleLicensePlate = licensePlate;
        command.VehicleLookup = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        return command.VehicleLookup != null;
    }

    private bool HaveEmailOrWhatsapp(CreateNotificationCommand command)
    {
        return !string.IsNullOrWhiteSpace(command.ReceiverEmailAddress) || !string.IsNullOrWhiteSpace(command.ReceiverWhatsappNumber);
    }

}
