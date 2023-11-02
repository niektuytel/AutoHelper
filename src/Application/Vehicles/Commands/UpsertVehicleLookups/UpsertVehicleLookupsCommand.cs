using AutoHelper.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;

public record UpsertVehicleLookupsCommand : IRequest
{
    public UpsertVehicleLookupsCommand()
    {
        MaxInsertAmount = 0;
        MaxUpdateAmount = 0;
        UpdateTimeline = true;
        UpdateServiceLogs = true;
    }

    public UpsertVehicleLookupsCommand(int maxInsertAmount)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = 0;
        UpdateTimeline = true;
        UpdateServiceLogs = true;
    }

    public UpsertVehicleLookupsCommand(int maxInsertAmount, int maxUpdateAmount, bool updateTimeline, bool updateServiceLogs)
    {
        MaxInsertAmount = maxInsertAmount;
        MaxUpdateAmount = maxUpdateAmount;
        UpdateTimeline = updateTimeline;
        UpdateServiceLogs = updateServiceLogs;
    }

    public int MaxInsertAmount { get; set; }
    public int MaxUpdateAmount { get; set; }
    public bool UpdateTimeline { get; set; }
    public bool UpdateServiceLogs { get; set; }
}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<UpsertVehicleLookupsCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVehicleService _vehicleService;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IMapper mapper, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _vehicleService = vehicleService;
    }

    public async Task<Unit> Handle(UpsertVehicleLookupsCommand request, CancellationToken cancellationToken)
    {
        var startMaxInsertAmount = request.MaxInsertAmount;
        var startMaxUpdateAmount = request.MaxUpdateAmount;


        // TODO: implement
        //var briefLookups = await _vehicleService.GetBriefVehicleLookups();

        //// only keep lookups with known services
        //var newLookups = briefLookups
        //    .Where(x => x.KnownServices.Any(y => y != VehicleServiceType.Other))
        //    .ToArray();

        //for (int i = 0; i < newLookups.Length; i++)
        //{
        //    var newLookup = newLookups[i];
        //    var currentLookups = _dbContext.VehicleLookups
        //        .Include(x => x.LargeData)
        //        .Where(x => x.Identifier == newLookup.Identifier.ToString());

        //    if (currentLookups.Count() > 1)
        //    { }
        //}

        return Unit.Value;
    }
}