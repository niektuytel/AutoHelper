using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;

public class CreateVehicleServiceLogAsGarageCommandValidator : AbstractValidator<CreateVehicleServiceLogAsGarageCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleServiceLogAsGarageCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .When(x => x.Type == GarageServiceType.Other);

        RuleFor(x => x.Date)
            .Must(ValidDate)
            .WithMessage("Invalid date format.")
            .DependentRules(() =>
            {
                RuleFor(x => x.ExpectedNextDate)
                    .Must(ValidDate)
                    .WithMessage("Invalid expected next date format.")
                    .When(x => !string.IsNullOrEmpty(x.ExpectedNextDate))
                    .DependentRules(() =>
                    {
                        RuleFor(x => x)
                            .Must(x => BeLaterDate(x))
                            .WithMessage("Expected next date must be later than the actual date.")
                            .When(x => !string.IsNullOrEmpty(x.ExpectedNextDate));
                    });
            });

        RuleFor(x => x.OdometerReading)
            .GreaterThanOrEqualTo(0).WithMessage("Odometer reading must be non-negative.");

        RuleFor(x => x.ExpectedNextOdometerReading)
            .GreaterThanOrEqualTo(x => x.OdometerReading)
            .WithMessage("Expected next odometer reading must be greater than or equal to the current odometer reading.")
            .When(x => x.ExpectedNextOdometerReading.HasValue);
    }

    private async Task<bool> BeValidAndExistingGarageLookup(CreateVehicleServiceLogAsGarageCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingVehicle(CreateVehicleServiceLogAsGarageCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.VehicleLicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        return vehicle != null;
    }

    private bool ValidDate(CreateVehicleServiceLogAsGarageCommand command, string date)
    {
        bool isValid = DateTime.TryParse(date, out var parsedDate);
        if (isValid)
        {
            // Set the parsed date
            if (date == command.Date)
                command.SetParsedDates(parsedDate, command.ParsedExpectedNextDate);
            else
                command.SetParsedDates(command.ParsedDate, parsedDate);
        }

        return isValid;
    }

    private bool BeLaterDate(CreateVehicleServiceLogAsGarageCommand command)
    {
        return command.ParsedExpectedNextDate > command.ParsedDate;
    }
}
