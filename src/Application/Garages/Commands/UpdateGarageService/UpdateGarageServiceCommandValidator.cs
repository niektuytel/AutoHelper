﻿using AutoHelper.Application.Garages._DTOs;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;

public class UpdateGarageServiceCommandValidator : AbstractValidator<UpdateGarageServiceCommand>
{
    public UpdateGarageServiceCommandValidator()
    {
        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");

        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type is required.");

        RuleFor(v => v.VehicleType)
            .NotNull().WithMessage("VehicleType is required.");

        RuleFor(v => v.VehicleFuelType)
            .NotNull().WithMessage("VehicleFuelType is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(800).WithMessage("Description must not exceed 800 characters.");

        RuleFor(v => v.ExpectedNextDateIsRequired)
            .NotNull().WithMessage("ExpectedNextDateIsRequired is required.");

        RuleFor(v => v.ExpectedNextOdometerReadingIsRequired)
            .NotNull().WithMessage("ExpectedNextOdometerReadingIsRequired is required.");

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
