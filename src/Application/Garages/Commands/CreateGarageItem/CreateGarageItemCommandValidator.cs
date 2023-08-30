using AutoHelper.Application.Common.Validators;
using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public class CreateGarageItemCommandValidator : AbstractValidator<CreateGarageItemCommand>
{
    public CreateGarageItemCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.Location)
            .SetValidator(new LocationValidator());

        RuleFor(v => v.BusinessOwner)
            .SetValidator(new BusinessOwnerValidator());

        RuleFor(v => v.BankingDetails)
            .SetValidator(new BankingInfoValidator());
    }
}


