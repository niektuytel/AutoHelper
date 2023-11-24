using FluentValidation;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;

public class GetVehicleTimelineQueryValidator : AbstractValidator<GetVehicleTimelineQuery>
{
    public GetVehicleTimelineQueryValidator()
    {
        // Validation rule for LicensePlate
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .Length(4, 9).WithMessage("License plate must be between 4 and 9 characters.")
            .Matches("^[A-Za-z0-9]+$").WithMessage("License plate must contain only letters and numbers.");

        // Validation rule for Take
        RuleFor(x => x.Take)
            .GreaterThanOrEqualTo(1).WithMessage("Take must be greater than or equal to 1.")
            .LessThanOrEqualTo(100).WithMessage("Take must be less than or equal to 100.");
    }
}