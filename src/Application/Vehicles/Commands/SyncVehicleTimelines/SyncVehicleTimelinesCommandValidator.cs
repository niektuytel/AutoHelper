using AutoHelper.Application.Vehicles.Queries.GetVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;

public class SyncVehicleTimelinesCommandValidator : AbstractValidator<SyncVehicleTimelinesCommand>
{
    public SyncVehicleTimelinesCommandValidator()
    {
        // Validation for StartRowIndex
        RuleFor(command => command.StartRowIndex)
            .GreaterThanOrEqualTo(SyncVehicleTimelinesCommand.DefaultStartingRowIndex)
            .WithMessage("Start row index must be greater than or equal to 0.");

        // Validation for EndRowIndex
        RuleFor(command => command.EndRowIndex)
            .GreaterThanOrEqualTo(SyncVehicleTimelinesCommand.DefaultEndingRowIndex)
            .WithMessage("End row index must be -1 or greater.");

        // Validation for MaxInsertAmount
        RuleFor(command => command.MaxInsertAmount)
            .GreaterThanOrEqualTo(SyncVehicleTimelinesCommand.InsertAll)
            .WithMessage("Max insert amount must be -1 or greater.");

        // Validation for MaxUpdateAmount
        RuleFor(command => command.MaxUpdateAmount)
            .GreaterThanOrEqualTo(SyncVehicleTimelinesCommand.UpdateAll)
            .WithMessage("Max update amount must be -1 or greater.");

        // Validation for BatchSize
        RuleFor(command => command.BatchSize)
            .GreaterThan(0)
            .WithMessage("Batch size must be greater than 0.");

        RuleFor(command => command.QueueService)
            .NotNull().WithMessage("Required to run in a QueueService");
    }
}