using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.DeleteGarageService;

public class DeleteGarageServiceCommandValidator : AbstractValidator<DeleteGarageServiceCommand>
{
    public DeleteGarageServiceCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");
    }
}
