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
        var garageLookups = _context.GarageLookups.ToArray();

        var newGarageLookups = await _garageInfoService.GetBriefGarageLookups();
        foreach (var newGarageLookup in newGarageLookups)
        {
            var identifier = newGarageLookup.Identifier.ToString();
            var existingGarageLookup = garageLookups.FirstOrDefault(x => x.Identifier == identifier);
            if (existingGarageLookup == null)
            {
                var detailedInfo = await _garageInfoService.UpdateByAddressAndCity(newGarageLookup);
                newGarageLookup.Latitude = 1;// TODO: Get from google maps api
                newGarageLookup.Longitude = 1;// TODO: Get from google maps api
                newGarageLookup.PhoneNumber = "1234567890";// TODO:// https://www.nldelphi.com/showthread.php?42479-Telefoonboek-goudengids-API
                _context.GarageLookups.Add(newGarageLookup);
            }
            else
            {
                existingGarageLookup.Identifier = newGarageLookup.Identifier;
                existingGarageLookup.Name = newGarageLookup.Name;
                existingGarageLookup.Address = newGarageLookup.Address;
                existingGarageLookup.City = newGarageLookup.City;
            }
        }

        // Kan ik meer informatie uit de RDW halen of ergenst anders vandaan?

        return Unit.Value;
    }
}
