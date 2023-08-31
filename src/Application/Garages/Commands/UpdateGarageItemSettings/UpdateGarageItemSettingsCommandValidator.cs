using AutoHelper.Application.Common.Validators;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;

public class UpdateGarageItemSettingsCommandValidator : AbstractValidator<UpdateGarageItemSettingsCommand>
{
    public UpdateGarageItemSettingsCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        When(v => string.IsNullOrEmpty(v.Name) == false, () =>
        {
            RuleFor(v => v.Name)
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");
        });
        
        When(v => v.Location != null, () =>
        {
            RuleFor(v => v.Location)
                .SetValidator(new LocationValidator());
        });

        When(v => v.BankingDetails != null, () =>
        {
            RuleFor(v => v.BankingDetails)
                .SetValidator(new BankingInfoValidator());
        });
    }
}
