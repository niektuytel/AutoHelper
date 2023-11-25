using AutoHelper.Application.Garages._DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;

public class UpdateGarageServiceCommandValidator : AbstractValidator<UpdateGarageServiceCommand>
{
    public UpdateGarageServiceCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        RuleFor(v => v.DurationInMinutes)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(v => v.Price)
            .NotEmpty().WithMessage("Price is required.");

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");
    }

    public class BriefBankingDetailsValidator : AbstractValidator<BriefBankingDetailsDtoItem>
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
