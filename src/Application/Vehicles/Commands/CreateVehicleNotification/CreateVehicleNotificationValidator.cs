using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public class CreateVehicleNotificationValidator : AbstractValidator<CreateVehicleNotificationCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleNotificationValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.ContactIdentifier)
            .NotEmpty().WithMessage("Either whatsapp number or email address is required.")
            .Must(contactIdentifier =>
                Regex.IsMatch(contactIdentifier, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$") || // Email format
                Regex.IsMatch(contactIdentifier, @"^\+?[0-9]{10,15}$") // Phone number format
            )
            .WithMessage("Invalid whatsapp number or email address.");

        RuleFor(x => x.ContactIdentifier)
            .MustAsync(BeUniqueNotification)
            .WithMessage("Er bestaat al een melding voor dit voertuig en deze contactpersoon.");
    }

    private async Task<bool> BeValidAndExistingVehicle(CreateVehicleNotificationCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");

        command.VehicleLicensePlate = licensePlate;
        command.VehicleLookup = await _context.VehicleLookups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        return command.VehicleLookup != null;
    }

    private async Task<bool> BeUniqueNotification(CreateVehicleNotificationCommand command, string? contactIdentifier, CancellationToken cancellationToken)
    {
        if (contactIdentifier == null)
        {
            return true;
        }

        var licensePlate = command.VehicleLicensePlate;
        licensePlate = licensePlate.ToUpper().Replace("-", "");

        var foundMatch = await _context.Notifications.AnyAsync(x =>
            x.VehicleLicensePlate == licensePlate &&
            x.ReceiverContactIdentifier.ToLower() == contactIdentifier.ToLower(), 
            cancellationToken
        );

        return foundMatch == false;
    }
}
