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
using Hangfire.Server;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Garages.Commands.UpsertGarageLookups;


public record UpsertGarageLookupsCommand : IRequest
{
    public UpsertGarageLookupsCommand()
    {
        MaxInsertAmount = 0;
        MaxUpdateAmount = 0;
    }

    public UpsertGarageLookupsCommand(int maxInsertAmount, int maxUpdateAmount)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
    }

    public int MaxInsertAmount { get; set;}
    public int MaxUpdateAmount { get; set;}
}

public class UpsertGarageLookupsCommandHandler : IRequestHandler<UpsertGarageLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IGarageInfoService _garageInfoService;
    private readonly IQueueingService _queueingService;

    public UpsertGarageLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IGarageInfoService garageInfoService, IQueueingService queueingService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _garageInfoService = garageInfoService;
        _queueingService = queueingService;
    }

    public async Task<Unit> Handle(UpsertGarageLookupsCommand request, CancellationToken cancellationToken)
    {
        var startMaxInsertAmount = request.MaxInsertAmount;
        var startMaxUpdateAmount = request.MaxUpdateAmount;
        var briefLookups = await _garageInfoService.GetBriefGarageLookups();

        // only keep lookups with known services
        var newLookups = briefLookups
            .Where(x => x.KnownServices.Any(y => y != GarageServiceType.Other))
            .ToArray();

        for (int i = 0; i < newLookups.Length; i++)
        {
            var newLookup = newLookups[i];
            var currentLookups = _dbContext.GarageLookups
                .Include(x => x.LargeData)
                .Where(x => x.Identifier == newLookup.Identifier.ToString());

            if(currentLookups.Count() > 1)
            {
                await Console.Out.WriteLineAsync(   );
            }

            var currentLookup = await currentLookups.FirstOrDefaultAsync(cancellationToken);
            if (request.MaxInsertAmount != 0 && currentLookup == null)
            {
                newLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                _dbContext.GarageLookups.Add(newLookup);

                await _dbContext.SaveChangesAsync(cancellationToken);
                request.MaxInsertAmount--;
            }
            else if (request.MaxUpdateAmount != 0 && currentLookup != null && currentLookup.Address != newLookup.Address)
            {
                currentLookup.Name = newLookup.Name;
                currentLookup.KnownServicesString = newLookup.KnownServicesString;
                currentLookup.Address = newLookup.Address;
                currentLookup.City = newLookup.City;

                currentLookup = await _garageInfoService.UpdateByAddressAndCity(newLookup);

                await _dbContext.SaveChangesAsync(cancellationToken);
                request.MaxUpdateAmount--;
            }

            if (request.MaxInsertAmount == 0 && request.MaxUpdateAmount == 0)
            {
                _queueingService.LogInformation($"Given MaxInsertAmount:{startMaxInsertAmount} MaxUpdateAmount:{startMaxUpdateAmount} reached");
                break;
            }
        }

        return Unit.Value;
    }
}
