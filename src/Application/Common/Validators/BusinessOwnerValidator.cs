using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Common.Validators;

// Validator for BusinessOwnerItem
public class BusinessOwnerValidator : AbstractValidator<BusinessOwnerItem>
{
    public BusinessOwnerValidator()
    {
        RuleFor(v => v.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .MaximumLength(200).WithMessage("FullName must not exceed 200 characters.");

        RuleFor(v => v.PhoneNumber)
            .Matches(@"^[0-9+\- \(\)]+$").WithMessage("PhoneNumber contains invalid characters.");

        RuleFor(v => v.Email)
            .EmailAddress().WithMessage("Invalid email format.");
    }
}

