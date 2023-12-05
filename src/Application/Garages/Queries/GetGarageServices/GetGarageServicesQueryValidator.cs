using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;

public class GetGarageServicesQueryValidator : AbstractValidator<GetGarageServicesQuery>
{
    private readonly IApplicationDbContext _context;

    public GetGarageServicesQueryValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetGarageServicesQuery command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }
}