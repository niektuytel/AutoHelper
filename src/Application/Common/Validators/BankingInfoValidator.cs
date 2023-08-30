using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Common.Validators;

// Validator for BankingInfoItem
public class BankingInfoValidator : AbstractValidator<BankingInfoItem>
{
    public BankingInfoValidator()
    {
        RuleFor(v => v.BankName)
            .NotEmpty().WithMessage("BankName is required.");

        RuleFor(v => v.AccountNumber)
            .NotEmpty().WithMessage("AccountNumber is required.");

        RuleFor(v => v.IBAN)
            .NotEmpty().WithMessage("IBAN is required."); // You may want a more detailed validation depending on your region's IBAN format.

        RuleFor(v => v.SWIFTCode)
            .NotEmpty().WithMessage("SWIFTCode is required."); // Consider more detailed validation if necessary.
    }
}

