using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Common.Validators;

// Validator for BankingInfoItem
public class BankingInfoValidator : AbstractValidator<GarageBankingDetailsItem>
{
    public BankingInfoValidator()
    {
        RuleFor(v => v.BankName)
            .NotEmpty().WithMessage("BankName is required.");

        RuleFor(v => v.KvKNumber)
            .NotEmpty().WithMessage("KvKNumber is required.");

        RuleFor(v => v.AccountHolderName)
            .NotEmpty().WithMessage("AccountHolderName is required.");

        RuleFor(v => v.IBAN)
            .NotEmpty().WithMessage("IBAN is required.");
    }
}

