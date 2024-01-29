using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;

public class CreateVehicleEventNotifierValidator : AbstractValidator<CreateVehicleEventNotifierCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleEventNotifierValidator(IApplicationDbContext applicationDbContext)
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
            );

    }

    private async Task<bool> BeValidAndExistingVehicle(CreateVehicleEventNotifierCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        
        command.VehicleLicensePlate = licensePlate;
        command.VehicleLookup = await _context.VehicleLookups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        
        return command.VehicleLookup != null;
    }
}
