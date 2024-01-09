using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;
using AutoHelper.Application.Vehicles._DTOs;
using FluentValidation;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookups;

public class GetGarageLookupCardsQueryValidator : AbstractValidator<GetGarageLookupsQuery>
{
    private readonly IVehicleService _vehicleService;

    public GetGarageLookupCardsQueryValidator(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;

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

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude is required.");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }

    private async Task<bool> BeValidAndExistingVehicleType(GetGarageLookupsQuery command, string licensePlate, CancellationToken cancellationToken)
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
