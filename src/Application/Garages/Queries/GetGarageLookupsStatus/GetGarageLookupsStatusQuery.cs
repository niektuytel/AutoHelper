using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookupStatus;
public class GetGarageLookupsStatusQuery : IRequest<GarageLookupsStatusDto>
{
}

public class GetGarageLookupsStatusQueryHandler : IRequestHandler<GetGarageLookupsStatusQuery, GarageLookupsStatusDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IGarageService _garageInfoService;

    public GetGarageLookupsStatusQueryHandler(IApplicationDbContext context, IGarageService garageInfoService)
    {
        _context = context;
        _garageInfoService = garageInfoService;
    }

    public async Task<GarageLookupsStatusDto> Handle(GetGarageLookupsStatusQuery request, CancellationToken cancellationToken)
    {
        var briefLookups = await _garageInfoService.GetBriefGarageLookups();

        // only keep lookups with known services
        var newLookups = briefLookups
            .Where(x => x.KnownServices.Any(y => y != GarageServiceType.Other))
            .ToArray();

        var status = new GarageLookupsStatusDto();
        for (int i = 0; i < newLookups.Length; i++)
        {
            var newLookup = newLookups[i];
            var currentLookups = _context.GarageLookups
                .Include(x => x.LargeData)
                .Where(x => x.Identifier == newLookup.Identifier.ToString());

            if (currentLookups.Count() > 1)
            {
                throw new Exception("Multiple lookups with same identifier");
            }

            var currentLookup = await currentLookups.FirstOrDefaultAsync();
            if (currentLookup == null)
            {
                status.AbleToInsert++;
            }
            else if (currentLookup.Address != newLookup.Address)
            {
                status.AbleToUpdate++;
            }
            else
            {
                status.UpToDate++;
            }

            status.Total++;
        }

        return status;
    }
}

