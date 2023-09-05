using AutoHelper.Application.Common.Validators;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageService;

public class UpdateGarageServiceCommandValidator : AbstractValidator<UpdateGarageServiceCommand>
{
    public UpdateGarageServiceCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.GarageId)
            .NotEmpty().WithMessage("GarageId is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        RuleFor(v => v.Duration)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(v => v.Price)
            .NotEmpty().WithMessage("Price is required.");

    }
}
