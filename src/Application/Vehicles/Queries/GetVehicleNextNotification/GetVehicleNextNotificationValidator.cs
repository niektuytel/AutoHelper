using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;

public class GetVehicleNextNotificationValidator : AbstractValidator<GetVehicleNextNotificationQuery>
{
    private readonly IApplicationDbContext _context;

    public GetVehicleNextNotificationValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

    }

    private async Task<bool> BeValidAndExistingVehicle(GetVehicleNextNotificationQuery command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.LicensePlate = licensePlate;

        if(command.Vehicle == null)
        {
            command.Vehicle = await _context.VehicleLookups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        }

        return command.Vehicle != null;
    }
}