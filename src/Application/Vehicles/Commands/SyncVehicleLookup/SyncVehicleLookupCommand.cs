using AutoHelper.Application.Common.Interfaces;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.SyncVehicleLookup;

public record SyncVehicleLookupCommand : IRequest<string>
{
    public SyncVehicleLookupCommand(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }

}

public class UpsertVehicleLookupsCommandHandler : IRequestHandler<SyncVehicleLookupCommand, string>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IVehicleService _vehicleService;

    public UpsertVehicleLookupsCommandHandler(IApplicationDbContext dbContext, IVehicleService vehicleService)
    {
        _dbContext = dbContext;
        _vehicleService = vehicleService;
    }

    public async Task<string> Handle(SyncVehicleLookupCommand request, CancellationToken cancellationToken)
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
            vehicleLookup = _vehicleService.CreateVehicleRecord(vehicle);
        }
        else
        {
            var hasChanges = _vehicleService.UpdateVehicleRecord(vehicle, vehicleLookup!);
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
}