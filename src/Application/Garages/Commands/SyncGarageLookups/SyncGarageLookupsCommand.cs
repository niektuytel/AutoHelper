using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.SyncGarageLookups;


public record SyncGarageLookupsCommand : IRequest
{
}

public class SyncGarageLookupsCommandHandler : IRequestHandler<SyncGarageLookupsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IGarageInfoService _garageInfoService;

    public SyncGarageLookupsCommandHandler(IApplicationDbContext context, IMapper mapper, IGarageInfoService garageInfoService)
    {
        _context = context;
        _mapper = mapper;
        _garageInfoService = garageInfoService;
    }

    public async Task<Unit> Handle(SyncGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        var briefLookups = await _garageInfoService.GetBriefGarageLookups();

        // only keep lookups with known services
        var newLookups = briefLookups
            .Where(x => x.KnownServices.Any(y => y != GarageServiceType.Other))
            .ToArray();

        for (int i = 0; i < newLookups.Length; i++)
        {
            var newLookup = newLookups[i];

            // not valid when having no name, city or address
            if(string.IsNullOrWhiteSpace(newLookup.Name) || string.IsNullOrWhiteSpace(newLookup.City) || string.IsNullOrWhiteSpace(newLookup.Address))
            {
                continue;
            }

            var currentLookup = _context.GarageLookups
                .Include(x => x.LargeData)
                .FirstOrDefault(x => x.Identifier == newLookup.Identifier.ToString());

            if (currentLookup == null)
            {
                newLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                _context.GarageLookups.Add(newLookup);

                await _context.SaveChangesAsync(cancellationToken);
            }
            else if (currentLookup.Address != newLookup.Address)
            {
                currentLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        return Unit.Value;
    }
}
