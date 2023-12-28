using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleTimeline;

public class SyncVehicleTimelineCommandValidator : AbstractValidator<SyncVehicleTimelineCommand>
{
    public SyncVehicleTimelineCommandValidator()
    {
        // Custom rule for processing and validating LicensePlate
        RuleFor(x => x.LicensePlate)
            .Custom((licensePlate, context) =>
            {
                // Validate if the license plate is not empty
                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    context.AddFailure("License plate is required.");
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
}