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

        var vehicleLookup = _dbContext.VehicleLookups.FirstOrDefault(v => v.LicensePlate == vehicle.LicensePlate);
        var onInsert = vehicleLookup == null;

        if (onInsert)
        {
            vehicleLookup = CreateVehicleRecord(vehicle);
        }
        else
        {
            var hasChanges = UpdateVehicleRecord(vehicle, vehicleLookup!);
            if (hasChanges == false)
            {
                return "Vehicle has no changed";
            }
        }

        if (onInsert)
        {
            _dbContext.VehicleLookups.Add(vehicleLookup!);
        }
        else
        {
            _dbContext.VehicleLookups.Update(vehicleLookup!);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return $"Done processing license:{request.LicensePlate}";
    }

    public static bool UpdateVehicleRecord(VehicleBasicsDtoItem? vehicle, VehicleLookupItem vehicleLookup)
    {
        bool somethingChanged = HasChangesRecord(vehicleLookup, vehicle);
        if (!somethingChanged)
        {
            return false;
        }

        // Update vehicleLookup details
        vehicleLookup.DateOfMOTExpiry = vehicle.MOTExpiryDateDt;
        vehicleLookup.DateOfAscription = vehicle.RegistrationDateDt;
        vehicleLookup.LastModified = DateTime.UtcNow;
        vehicleLookup.LastModifiedBy = $"system";

        return true;
    }

    private static bool HasChangesRecord(VehicleLookupItem vehicleLookup, VehicleBasicsDtoItem vehicle)
    {
        var sameExpirationDate = vehicleLookup.DateOfMOTExpiry == vehicle.MOTExpiryDateDt;
        var sameRegistrationDate = vehicleLookup.DateOfAscription == vehicle.RegistrationDateDt;
        if (sameExpirationDate && sameRegistrationDate)
        {
            return false;
        }

        return true;
    }

    public static VehicleLookupItem CreateVehicleRecord(VehicleBasicsDtoItem vehicle)
    {
        var vehicleLookup = new VehicleLookupItem
        {
            LicensePlate = vehicle.LicensePlate,
            DateOfMOTExpiry = vehicle.MOTExpiryDateDt,
            DateOfAscription = vehicle.RegistrationDateDt,
            Created = DateTime.UtcNow,
            CreatedBy = $"system",
            LastModified = DateTime.UtcNow,
            LastModifiedBy = $"system"
        };

        return vehicleLookup;
    }


}