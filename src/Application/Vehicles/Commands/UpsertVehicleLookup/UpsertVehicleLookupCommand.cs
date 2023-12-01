using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using Force.DeepCloner;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;

public record UpsertVehicleLookupCommand : IRequest<string>
{
    public UpsertVehicleLookupCommand(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }

}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<UpsertVehicleLookupCommand, string>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IVehicleService _vehicleService;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _vehicleService = vehicleService;
    }

    public async Task<string> Handle(UpsertVehicleLookupCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleService.GetBasicVehicle(request.LicensePlate);
        if (vehicle == null)
        {
            return "Vehicle does not exist";
        }

        // remove existing lookup
        var dbVehicle = _dbContext.VehicleLookups.FirstOrDefault(v => v.LicensePlate == request.LicensePlate);
        if (dbVehicle != null)
        {
            _dbContext.VehicleLookups.Remove(dbVehicle);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        // create new lookup
        dbVehicle = new VehicleLookupItem
        {
            LicensePlate = vehicle.LicensePlate,
            DateOfMOTExpiry = vehicle.MOTExpiryDateDt,
            DateOfAscription = vehicle.RegistrationDateDt,
            Created = DateTime.UtcNow,
            CreatedBy = $"system",
            LastModified = DateTime.UtcNow,
            LastModifiedBy = $"system"
        };

        _dbContext.VehicleLookups.Add(dbVehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"Done processing license:{request.LicensePlate}";
    }

}