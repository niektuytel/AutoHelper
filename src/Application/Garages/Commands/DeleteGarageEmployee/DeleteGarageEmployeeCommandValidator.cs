using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.DeleteGarageEmployee;

public class DeleteGarageEmployeeCommandValidator : AbstractValidator<DeleteGarageEmployeeCommand>
{
    public DeleteGarageEmployeeCommandValidator()
    {
        //RuleFor(v => v.Id)
        //    .NotEmpty().WithMessage("Id is required.");

        //RuleFor(v => v.Type)
        //    .NotEmpty().WithMessage("Type is required.");

        //RuleFor(v => v.Description)
        //    .NotEmpty().WithMessage("Description is required.")
        //    .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        //RuleFor(v => v.DurationInMinutes)
        //    .NotEmpty().WithMessage("Duration is required.");

        //RuleFor(v => v.Price)
        //    .NotEmpty().WithMessage("Price is required.");

    }
}
