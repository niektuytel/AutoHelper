using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Garages;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;

public class GetGarageServicesAsVehicleQueryValidator : AbstractValidator<GetGarageServicesAsVehicleQuery>
{
    private readonly IApplicationDbContext _context;
    private readonly IVehicleService _vehicleService;

    public GetGarageServicesAsVehicleQueryValidator(IApplicationDbContext context, IVehicleService vehicleService)
    {
        _context = context;
        _vehicleService = vehicleService;

        // Custom rule for processing and validating LicensePlate
        RuleFor(x => x.LicensePlate)
            .Custom((licensePlate, context) =>
            {
                // Ignore when no license plate is provided
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
            })
            .MustAsync(BeValidAndExistingVehicleType)
            .WithMessage("No vehicle lookup found for this Licenseplate."); ;

        RuleFor(v => v.GarageLookupIdentifier)
            .NotEmpty()
            .WithMessage("GarageLookupIdentifier cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage lookup found for this GarageLookupIdentifier.");

    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetGarageServicesAsVehicleQuery command, string? identifier, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageLookups
            .FirstOrDefaultAsync(x => x.Identifier == identifier, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageLookupItem), identifier ?? "");
        }

        command.GarageLookup = entity;
        return true;
    }

    private async Task<bool> BeValidAndExistingVehicleType(GetGarageServicesAsVehicleQuery command, string licensePlate, CancellationToken cancellationToken)
    {
        var entity = await _vehicleService.GetVehicleByLicensePlateAsync(licensePlate);
        if (entity == null)
        {
            throw new NotFoundException(nameof(VehicleSpecificationsCardItem), licensePlate ?? "");
        }

        command.VehicleType = entity.Type;
        return true;
    }
}