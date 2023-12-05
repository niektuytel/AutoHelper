using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;
using AutoHelper.Domain.Entities.Conversations.Enums;

namespace AutoHelper.Application.Common.Interfaces;

public interface IGarageService
{
    Task<GarageLookupItem[]> GetBriefGarageLookups();
    Task<GarageLookupItem> SetConversationSettings(string garageIdentifier, string contactIdentifier, ContactType contactType, GarageServiceType[] services, CancellationToken cancellationToken);
    Task<GarageLookupItem> UpdateByLocation(GarageLookupItem item);
}