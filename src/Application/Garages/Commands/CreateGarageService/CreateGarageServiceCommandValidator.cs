using AutoHelper.Application.Common.Validators;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;

public class CreateGarageServiceCommandValidator : AbstractValidator<CreateGarageServiceCommand>
{
    public CreateGarageServiceCommandValidator()
    {
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


