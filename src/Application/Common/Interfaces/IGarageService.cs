using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Application.Garages._DTOs;

namespace AutoHelper.Application.Common.Interfaces;

public interface IGarageService
{
    Task<IEnumerable<RDWCompany>> GetRDWCompanies(int offset, int limit);
    Task<int> GetRDWCompaniesCount();
    Task<IEnumerable<RDWCompanyService>> GetRDWServices();
    Task<(GarageLookupItem? itemToInsert, GarageLookupItem? itemToUpdate)> UpsertLookup(GarageLookupItem? garage, RDWCompany company);
    Task<(List<GarageLookupServiceItem> itemsToInsert,List<GarageLookupServiceItem> itemsToRemove)> UpsertLookupServices(IEnumerable<GarageLookupServiceItem>? garageServices, IEnumerable<GarageLookupServiceItem> rdwServices, string garageIdentifier);
}