using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.UpdateVehicleServiceLogAsGarage;

public class UpdateVehicleServiceLogAsGarageCommandValidator : AbstractValidator<UpdateVehicleServiceLogAsGarageCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateVehicleServiceLogAsGarageCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
        CascadeMode = CascadeMode.Stop;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(command => command.Id)
            .MustAsync(BeValidAndExistingServiceLog)
            .WithMessage("Service log does not exist under this garage.");

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        // Date & ExpectedNextDate
        ValidateDate();
        ValidateOdometerReadings();
        ValidateExpectedNextDateAndOdometerReading();

    }

    private async Task<bool> BeValidAndExistingGarageLookup(UpdateVehicleServiceLogAsGarageCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingServiceLog(UpdateVehicleServiceLogAsGarageCommand command, Guid logId, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleServiceLogs
            .FirstOrDefaultAsync(x =>
                x.GarageLookupIdentifier == command.Garage.GarageLookupIdentifier &&
                x.Id == logId,
                cancellationToken
            );

        command.ServiceLog = entity;
        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingVehicle(UpdateVehicleServiceLogAsGarageCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.VehicleLicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        return vehicle != null;
    }

    private bool ValidDate(UpdateVehicleServiceLogAsGarageCommand command, string date)
    {
        bool isValid = DateTime.TryParse(date, out var parsedDate);
        if (isValid)
        {
            // Set the parsed date
            if (date == command.Date)
                command.SetParsedDates(parsedDate, command.ParsedExpectedNextDate);
            else
                command.SetParsedDates(command.ParsedDate, parsedDate);
        }

        return isValid;
    }

    private void ValidateDate()
    {
        RuleFor(x => x.Date)
            .Must(ValidDate).WithMessage("Invalid date format.");
    }

    private void ValidateOdometerReadings()
    {
        RuleFor(x => x.OdometerReading)
            .GreaterThanOrEqualTo(0).WithMessage("Odometer reading must be non-negative.");

        RuleFor(x => x.ExpectedNextOdometerReading)
            .GreaterThanOrEqualTo(x => x.OdometerReading)
            .WithMessage("Expected next odometer reading must be greater than or equal to the current odometer reading.")
            .When(x => x.ExpectedNextOdometerReading.HasValue);
    }

    private void ValidateExpectedNextDateAndOdometerReading()
    {
        RuleFor(x => x)
            .Custom(ValidateExpectedNextDate)
            .Custom(ValidateExpectedNextOdometerReading)
            .CustomAsync(ValidateOdometerReadingConsistency);
    }

    private bool ValidDate(string date, out DateTime parsedDate)
    {
        return DateTime.TryParse(date, out parsedDate);
    }

    private void ValidateExpectedNextDate(UpdateVehicleServiceLogAsGarageCommand command, ValidationContext<UpdateVehicleServiceLogAsGarageCommand> context)
    {
        if (!string.IsNullOrEmpty(command.ExpectedNextDate))
        {
            if (!ValidDate(command.ExpectedNextDate, out var parsedNextDate))
            {
                context.AddFailure("ExpectedNextDate", "Invalid expected next date format.");
            }
            else if (parsedNextDate < command.ParsedDate)
            {
                context.AddFailure("ExpectedNextDate", "Expected next date must be later than the actual date.");
            }
        }
    }

    private void ValidateExpectedNextOdometerReading(UpdateVehicleServiceLogAsGarageCommand command, ValidationContext<UpdateVehicleServiceLogAsGarageCommand> context)
    {
        if (command.ExpectedNextOdometerReading.HasValue && command.ExpectedNextOdometerReading < command.OdometerReading)
        {
            context.AddFailure("ExpectedNextOdometerReading", "Expected next odometer reading must be greater than current reading.");
        }
    }

    private async Task ValidateOdometerReadingConsistency(UpdateVehicleServiceLogAsGarageCommand command, ValidationContext<UpdateVehicleServiceLogAsGarageCommand> context, CancellationToken cancellationToken)
    {
        if (command.OdometerReading != default)
        {
            var existingEntries = await _context.VehicleServiceLogs
                .Where(vl => vl.VehicleLicensePlate == command.VehicleLicensePlate)
                .ToListAsync(cancellationToken);

            var largerOdoButSmalLerDate = existingEntries.Where(x => x.OdometerReading > command.OdometerReading && x.Date < command.ParsedDate);
            if (largerOdoButSmalLerDate.Any())
            {
                context.AddFailure("OdometerReading", $"Er zijn hogere KM-standen bekend dan {command.OdometerReading} voor de datum {command.ParsedDate!.Value.ToShortDateString()}");
            }

            var smallerOdoButLargerDate = existingEntries.Where(x => x.OdometerReading < command.OdometerReading && x.Date > command.ParsedDate);
            if (smallerOdoButLargerDate.Any())
            {
                context.AddFailure("OdometerReading", $"Er zijn lagere KM-standen bekend dan {command.OdometerReading} na de datum {command.ParsedDate!.Value.ToShortDateString()}");
            }
        }
    }
}
