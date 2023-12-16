﻿using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookup;

public class GetGarageLookupQueryValidator : AbstractValidator<GetGarageLookupQuery>
{
    private readonly IApplicationDbContext _context;

    public GetGarageLookupQueryValidator(IApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(v => v.GarageLookupIdentifier)
            .NotEmpty()
            .WithMessage("GarageLookupIdentifier cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage lookup found for this GarageLookupIdentifier.");

    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetGarageLookupQuery command, string? identifier, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageLookups
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.Identifier == identifier, cancellationToken);

        command.GarageLookup = entity;
        return command.GarageLookup != null;
    }
}
