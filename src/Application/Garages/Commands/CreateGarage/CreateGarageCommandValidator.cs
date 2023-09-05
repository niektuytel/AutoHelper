using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public class CreateGarageCommandValidator : AbstractValidator<CreateGarageCommand>
{
    public CreateGarageCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.");

        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(v => v.Location)
            .SetValidator(new BriefLocationValidator());

        RuleFor(v => v.BankingDetails)
            .SetValidator(new BriefBankingDetailsValidator());
    }

    public class BriefLocationValidator : AbstractValidator<BriefLocationDto>
    {
        public BriefLocationValidator()
        {
            RuleFor(v => v.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(v => v.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(v => v.Country)
                .NotEmpty().WithMessage("Country is required.");

            RuleFor(v => v.Longitude)
                .NotEmpty().WithMessage("Longitude is required.");

            RuleFor(v => v.Latitude)
                .NotEmpty().WithMessage("Latitude is required.");

        }
    }

    public class BriefBankingDetailsValidator : AbstractValidator<BriefBankingDetailsDto>
    {
        public BriefBankingDetailsValidator()
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

}


