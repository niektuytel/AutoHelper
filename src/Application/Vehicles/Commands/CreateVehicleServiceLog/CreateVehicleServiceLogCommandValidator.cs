using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public class CreateVehicleServiceLogCommandValidator : AbstractValidator<CreateVehicleServiceLogCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateVehicleServiceLogCommandValidator(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _context = applicationDbContext;
        _mapper = mapper;

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.GarageLookupIdentifier)
            .NotEmpty().WithMessage("Garage identifier is required.")
            .MustAsync(BeValidAndExistingGarage)
            .WithMessage("Invalid or non-existent garage."); ;

        RuleFor(x => x.GarageServiceId)
            .NotEmpty().WithMessage("Garage service ID is required.")
            .MustAsync(BeValidAndExistingGarageService)
            .WithMessage("Invalid or non-existent garage service.");

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

        RuleFor(x => x.ReporterName)
            .NotEmpty().WithMessage("Created by is required.");

        RuleFor(x => x.ReporterPhoneNumber)
            .Matches(@"^\+?[0-9]+$").WithMessage("Invalid phone number format.")
            .When(x => !string.IsNullOrEmpty(x.ReporterPhoneNumber));

        RuleFor(x => x.ReporterEmailAddress)
            .EmailAddress().WithMessage("Invalid email address format.")
            .When(x => !string.IsNullOrEmpty(x.ReporterEmailAddress));

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.ReporterPhoneNumber) || !string.IsNullOrEmpty(x.ReporterEmailAddress))
            .WithMessage("Either phone number or email address is required.");

    }

    private async Task<bool> BeValidAndExistingVehicle(CreateVehicleServiceLogCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.VehicleLicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        return vehicle != null;
    }

    private async Task<bool> BeValidAndExistingGarage(CreateVehicleServiceLogCommand command, string lookupIdentifier, CancellationToken cancellationToken)
    {
        var garage = await _context.GarageLookups.FirstOrDefaultAsync(x => x.Identifier == lookupIdentifier, cancellationToken);
        command.Garage = garage;

        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingGarageService(CreateVehicleServiceLogCommand command, Guid? garageServiceId, CancellationToken cancellationToken)
    {
        var value = await _context.GarageServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == garageServiceId,
                cancellationToken: cancellationToken
            );

        GarageServiceDtoItem result = null!;
        if (value == null)
        {
            var otherValue = await _context.GarageLookupServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == garageServiceId,
                cancellationToken: cancellationToken
            );

            result = _mapper.Map<GarageServiceDtoItem>(otherValue);
        }
        else
        {
            result = _mapper.Map<GarageServiceDtoItem>(value);
        }

        command.GarageService = result;
        return command.GarageService != null;
    }

    private bool ValidDate(CreateVehicleServiceLogCommand command, string date)
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

    private bool BeLaterDate(CreateVehicleServiceLogCommand command)
    {
        return command.ParsedExpectedNextDate > command.ParsedDate;
    }
}
