using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;

public class CreateVehicleServiceLogAsGarageCommandValidator : AbstractValidator<CreateVehicleServiceLogAsGarageCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleServiceLogAsGarageCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
        CascadeMode = CascadeMode.Stop;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.GarageServiceId)
            .NotEmpty().WithMessage("Garage service ID is required.")
            .MustAsync(BeValidAndExistingGarageService)
            .WithMessage("Invalid or non-existent garage service.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        // Date & ExpectedNextDate
        ValidateDate();
        ValidateOdometerReadings();
        ValidateExpectedNextDateAndOdometerReading();

    }

    private async Task<bool> BeValidAndExistingGarageLookup(CreateVehicleServiceLogAsGarageCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingGarageService(CreateVehicleServiceLogAsGarageCommand command, Guid? garageServiceId, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == garageServiceId,
                cancellationToken: cancellationToken
            );

        command.GarageService = entity;
        return command.GarageService != null;
    }

    private async Task<bool> BeValidAndExistingVehicle(CreateVehicleServiceLogAsGarageCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.VehicleLicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        return vehicle != null;
    }

    private bool ValidDate(CreateVehicleServiceLogAsGarageCommand command, string date)
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

    private void ValidateExpectedNextDate(CreateVehicleServiceLogAsGarageCommand command, ValidationContext<CreateVehicleServiceLogAsGarageCommand> context)
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

    private void ValidateExpectedNextOdometerReading(CreateVehicleServiceLogAsGarageCommand command, ValidationContext<CreateVehicleServiceLogAsGarageCommand> context)
    {
        if (command.ExpectedNextOdometerReading.HasValue && command.ExpectedNextOdometerReading < command.OdometerReading)
        {
            context.AddFailure("ExpectedNextOdometerReading", "Expected next odometer reading must be greater than current reading.");
        }
    }

    private async Task ValidateOdometerReadingConsistency(CreateVehicleServiceLogAsGarageCommand command, ValidationContext<CreateVehicleServiceLogAsGarageCommand> context, CancellationToken cancellationToken)
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
