using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;

public class UpdateGarageSettingsCommandValidator : AbstractValidator<UpdateGarageSettingsCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGarageSettingsCommandValidator(IApplicationDbContext context)
    {
        _context = context;

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

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(async (userId, cancellationToken) =>
            {
                return await _context.Garages.AnyAsync(x => x.UserId == userId, cancellationToken);
            })
            .WithMessage("No garage found for this user.");
    }

    public class BriefLocationValidator : AbstractValidator<GarageLocationItem>
    {
        public BriefLocationValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Id is required.");

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

    public class BriefBankingDetailsValidator : AbstractValidator<GarageBankingDetailsItem>
    {
        public BriefBankingDetailsValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty().WithMessage("Id is required.");

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
