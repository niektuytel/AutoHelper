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

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;


public record UpsertGarageLookupsCommand : IRequest
{
    public UpsertGarageLookupsCommand(int maxInsertAmount = -1, int maxUpdateAmount = -1)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
    }

    public int MaxInsertAmount { get; set;}
    public int MaxUpdateAmount { get; set;}
}

public class UpsertGarageLookupsCommandHandler : IRequestHandler<UpsertGarageLookupsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IGarageInfoService _garageInfoService;

    public UpsertGarageLookupsCommandHandler(IApplicationDbContext context, IMapper mapper, IGarageInfoService garageInfoService)
    {
        _context = context;
        _mapper = mapper;
        _garageInfoService = garageInfoService;
    }

    public async Task<Unit> Handle(UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        var briefLookups = await _garageInfoService.GetBriefGarageLookups();

        // only keep lookups with known services
        var newLookups = briefLookups
            .Where(x => x.KnownServices.Any(y => y != GarageServiceType.Other))
            .ToArray();

        for (int i = 0; i < newLookups.Length; i++)
        {
            var newLookup = newLookups[i];
            var currentLookup = _context.GarageLookups
                .Include(x => x.LargeData)
                .FirstOrDefault(x => x.Identifier == newLookup.Identifier.ToString());

            if (request.MaxInsertAmount != 0 && currentLookup == null)
            {
                newLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                _context.GarageLookups.Add(newLookup);

                await _context.SaveChangesAsync(cancellationToken);
                request.MaxInsertAmount--;
            }
            else if (request.MaxUpdateAmount != 0 && currentLookup != null && currentLookup.Address != newLookup.Address)
            {
                currentLookup.Address = newLookup.Address;
                currentLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                await _context.SaveChangesAsync(cancellationToken);
                request.MaxUpdateAmount--;
            }

            if (request.MaxInsertAmount == 0 && request.MaxUpdateAmount == 0)
            {
                //TODO: log not showing to hangfre
                Console.WriteLine($"Given MaxInsertAmount({request.MaxInsertAmount})/MaxUpdateAmount({request.MaxUpdateAmount}) reached");
                break;
            }
        }

        return Unit.Value;
    }
}
