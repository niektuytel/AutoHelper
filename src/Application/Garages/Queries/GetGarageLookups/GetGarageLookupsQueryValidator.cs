using FluentValidation;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public class GetGarageLookupsQueryValidator : AbstractValidator<GetGarageLookupsQuery>
{
    public GetGarageLookupsQueryValidator()
    {
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("LicensePlate is required.");

        RuleFor(x => x.Latitude)
            .NotEmpty().WithMessage("Latitude is required.");

        RuleFor(x => x.Longitude)
            .NotEmpty().WithMessage("Longitude is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}
