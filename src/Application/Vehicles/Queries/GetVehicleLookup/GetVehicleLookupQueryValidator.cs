using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleLookup;

public class GetVehicleLookupQueryValidator : AbstractValidator<GetVehicleLookupQuery>
{
    public GetVehicleLookupQueryValidator()
    {
        // Validation rule for LicensePlate
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .Length(4, 9).WithMessage("License plate must be between 4 and 9 characters.")
            .Matches("^[A-Za-z0-9]+$").WithMessage("License plate must contain only letters and numbers.");

    }
}