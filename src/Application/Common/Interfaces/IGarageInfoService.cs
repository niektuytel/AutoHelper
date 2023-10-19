using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Common.Interfaces;

public interface IGarageInfoService
{
    int CalculateDistanceInKm(float garageLatitude, float garageLongitude, float latitude, float longitude);
    Task<GarageLookupItem[]> GetBriefGarageLookups();
    IEnumerable<GarageServiceType> GetRelatedServiceTypes(VehicleType vehicleType);
    Task<GarageLookupItem> UpdateByAddressAndCity(GarageLookupItem item);
}