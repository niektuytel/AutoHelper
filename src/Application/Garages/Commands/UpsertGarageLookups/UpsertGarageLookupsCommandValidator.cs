﻿using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;
using AutoHelper.Application.Vehicles.Queries.GetVehicleLookup;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;

public class UpsertGarageLookupsCommandValidator : AbstractValidator<UpsertGarageLookupsCommand>
{
    public UpsertGarageLookupsCommandValidator()
    {
        // Validation for StartRowIndex
        RuleFor(command => command.StartRowIndex)
            .GreaterThanOrEqualTo(UpsertGarageLookupsCommand.DefaultStartingRowIndex)
            .WithMessage("Start row index must be greater than or equal to 0.");

        // Validation for EndRowIndex
        RuleFor(command => command.EndRowIndex)
            .GreaterThanOrEqualTo(UpsertGarageLookupsCommand.DefaultEndingRowIndex)
            .WithMessage("End row index must be -1 or greater.")
            .When(command => command.EndRowIndex != UpsertGarageLookupsCommand.DefaultEndingRowIndex)
            .GreaterThanOrEqualTo(command => command.StartRowIndex)
            .WithMessage("End row index must be greater than or equal to start row index.");

        // Validation for MaxInsertAmount
        RuleFor(command => command.MaxInsertAmount)
            .GreaterThanOrEqualTo(UpsertGarageLookupsCommand.InsertAll)
            .WithMessage("Max insert amount must be -1 or greater.");

        // Validation for MaxUpdateAmount
        RuleFor(command => command.MaxUpdateAmount)
            .GreaterThanOrEqualTo(UpsertGarageLookupsCommand.UpdateAll)
            .WithMessage("Max update amount must be -1 or greater.");

        // Validation for BatchSize
        RuleFor(command => command.BatchSize)
            .GreaterThan(0)
            .WithMessage("Batch size must be greater than 0.");

        RuleFor(command => command.QueueService)
            .NotNull().WithMessage("Required to run in a QueueService");
    }
}