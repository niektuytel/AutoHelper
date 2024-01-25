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

        RuleFor(x => x.ReceiverWhatsappNumber)
            .Matches(@"^\+?[0-9]+$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.ReceiverWhatsappNumber));

        RuleFor(x => x.ReceiverEmailAddress)
            .EmailAddress().WithMessage("Invalid email address format.")
            .When(x => !string.IsNullOrEmpty(x.ReceiverEmailAddress));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.ReceiverWhatsappNumber) || !string.IsNullOrEmpty(x.ReceiverEmailAddress))
            .WithMessage("Either whatsapp number or email address is required.");

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
