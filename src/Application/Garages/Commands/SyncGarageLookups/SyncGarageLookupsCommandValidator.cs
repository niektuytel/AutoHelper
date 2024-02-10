using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;

public class SyncGarageLookupsCommandValidator : AbstractValidator<SyncGarageLookupsCommand>
{
    public SyncGarageLookupsCommandValidator()
    {
        // Validation for StartRowIndex
        RuleFor(command => command.StartRowIndex)
            .GreaterThanOrEqualTo(SyncGarageLookupsCommand.DefaultStartingRowIndex)
            .WithMessage("Start row index must be greater than or equal to 0.");

        // Validation for EndRowIndex
        RuleFor(command => command.EndRowIndex)
            .GreaterThanOrEqualTo(SyncGarageLookupsCommand.DefaultEndingRowIndex)
            .WithMessage("End row index must be -1 or greater.");

        // Validation for MaxInsertAmount
        RuleFor(command => command.MaxInsertAmount)
            .GreaterThanOrEqualTo(SyncGarageLookupsCommand.InsertAll)
            .WithMessage("Max insert amount must be -1 or greater.");

        // Validation for MaxUpdateAmount
        RuleFor(command => command.MaxUpdateAmount)
            .GreaterThanOrEqualTo(SyncGarageLookupsCommand.UpdateAll)
            .WithMessage("Max update amount must be -1 or greater.");

        // Validation for BatchSize
        RuleFor(command => command.BatchSize)
            .GreaterThan(0)
            .WithMessage("Batch size must be greater than 0.");

        RuleFor(command => command.QueueingService)
            .NotNull().WithMessage("Required to run in a QueueService");
    }
}