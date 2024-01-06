using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using AutoHelper.Domain.Entities.Garages;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageOverview;

public class GetGarageOverviewQueryValidator : AbstractValidator<GetGarageOverviewQuery>
{
    private readonly IApplicationDbContext _context;

    public GetGarageOverviewQueryValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetGarageOverviewQuery command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), userId ?? "");
        }

        command.Garage = entity;
        return true;
    }
}