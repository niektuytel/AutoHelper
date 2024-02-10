using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLogAsGarage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

public class DeleteVehicleServiceLogAsGarageCommandValidator : AbstractValidator<DeleteVehicleServiceLogAsGarageCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteVehicleServiceLogAsGarageCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(command => command.ServiceLogId)
            .NotEmpty().WithMessage("The ID must not be empty")
            .NotEqual(Guid.Empty).WithMessage("The ID must not be a default GUID")
            .MustAsync(BeValidAndExistingServiceLog)
            .WithMessage("Service log does not exist under this garage."); ;
    }

    private async Task<bool> BeValidAndExistingGarageLookup(DeleteVehicleServiceLogAsGarageCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

    private async Task<bool> BeValidAndExistingServiceLog(DeleteVehicleServiceLogAsGarageCommand command, Guid logId, CancellationToken cancellationToken)
    {
        var entity = await _context.VehicleServiceLogs
            .FirstOrDefaultAsync(x =>
                x.GarageLookupIdentifier == command.Garage.GarageLookupIdentifier && x.Id == logId,
                cancellationToken
            );

        command.ServiceLog = entity;
        return command.Garage != null;
    }

}
