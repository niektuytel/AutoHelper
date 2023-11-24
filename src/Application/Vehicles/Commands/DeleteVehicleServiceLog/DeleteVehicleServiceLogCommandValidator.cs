using FluentValidation;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLog;

public class DeleteVehicleServiceLogCommandValidator : AbstractValidator<DeleteVehicleServiceLogCommand>
{
    public DeleteVehicleServiceLogCommandValidator()
    {
        RuleFor(command => command.Id)
            .NotEmpty().WithMessage("The ID must not be empty")
            .NotEqual(Guid.Empty).WithMessage("The ID must not be a default GUID");
    }
}
