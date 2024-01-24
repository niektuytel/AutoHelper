using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleEventNotifier;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleEventNotifier;

public class DeleteVehicleEventNotifierValidator : AbstractValidator<DeleteVehicleEventNotifierCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteVehicleEventNotifierValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(x => x)
            .MustAsync(BeValidAndExistingIdentifier)
            .WithMessage("Invalid or non-existent Notification on identifier");

    }

    private async Task<bool> BeValidAndExistingIdentifier(DeleteVehicleEventNotifierCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        command.Notification = entity;
        return entity != null;
    }
}
