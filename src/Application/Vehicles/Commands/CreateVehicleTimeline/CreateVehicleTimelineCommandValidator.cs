using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;

public class CreateVehicleTimelineCommandValidator : AbstractValidator<CreateVehicleTimelineCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleTimelineCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(x => x.ServiceLog.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.");
    }
}
