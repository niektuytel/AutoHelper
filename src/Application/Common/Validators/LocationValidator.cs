using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Common.Validators;

// Validator for LocationItem
public class LocationValidator : AbstractValidator<LocationItem>
{
    public LocationValidator()
    {
        RuleFor(v => v.Longitude)
            .NotEmpty().WithMessage("Longitude is required.");

        RuleFor(v => v.Latitude)
            .NotEmpty().WithMessage("Latitude is required.");

        RuleFor(v => v.City)
            .MaximumLength(100).WithMessage("City must not exceed 100 characters.");
    }
}

