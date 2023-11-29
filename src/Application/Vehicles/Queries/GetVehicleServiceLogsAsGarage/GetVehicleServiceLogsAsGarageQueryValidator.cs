using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.UpdateVehicleServiceLogAsGarage;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogsAsGarage;

public class GetVehicleServiceLogsAsGarageQueryValidator : AbstractValidator<GetVehicleServiceLogsAsGarageQuery>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleServiceLogsAsGarageQueryValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");


        // Custom rule for processing and validating LicensePlate
        RuleFor(x => x.LicensePlate)
            .Custom((licensePlate, context) =>
            {
                // Not required, show all vehicles if empty
                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    return;
                }

                // Replace spaces or hyphens with an empty string
                var processedLicensePlate = licensePlate.Replace(" ", "").Replace("-", "");

                // Validate the length of the processed license plate
                if (processedLicensePlate.Length < 4 || processedLicensePlate.Length > 9)
                {
                    context.AddFailure("License plate must be between 4 and 9 characters.");
                }
                // Validate the character content of the processed license plate
                else if (!processedLicensePlate.All(char.IsLetterOrDigit))
                {
                    context.AddFailure("License plate must contain only letters and numbers.");
                }
                else
                {
                    // Update the license plate in the context if it passes validation
                    context.InstanceToValidate.LicensePlate = processedLicensePlate;
                }
            });
    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetVehicleServiceLogsAsGarageQuery command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

}