using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;

public class UpdateGarageEmployeeCommandValidator : AbstractValidator<UpdateGarageEmployeeCommand>
{
    public UpdateGarageEmployeeCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("UserId is required.");

    }
}
