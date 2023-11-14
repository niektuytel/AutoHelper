using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public class CreateVehicleServiceLogCommandValidator : AbstractValidator<CreateVehicleServiceLogCommand>
{
    public CreateVehicleServiceLogCommandValidator()
    {
        //RuleFor(v => v.Name)
        //    .NotEmpty().WithMessage("Name is required.")
        //    .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        //RuleFor(v => v.PhoneNumber)
        //    .NotEmpty().WithMessage("PhoneNumber is required.");

        //RuleFor(v => v.Email)
        //    .NotEmpty().WithMessage("Email is required.");

        //RuleFor(v => v.Location)
        //    .SetValidator(new BriefLocationValidator());

        //RuleFor(v => v.BankingDetails)
        //    .SetValidator(new BriefBankingDetailsValidator());
    }

}


