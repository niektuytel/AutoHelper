using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Common.Interfaces;

public interface IGarageService
{
    int CalculateDistanceInKm(float garageLatitude, float garageLongitude, float latitude, float longitude);
    Task<GarageLookupItem[]> GetBriefGarageLookups();
    IEnumerable<GarageServiceType> GetRelatedServiceTypes(VehicleLookupType vehicleType);
    Task<GarageLookupItem> UpdateByAddressAndCity(GarageLookupItem item);
}