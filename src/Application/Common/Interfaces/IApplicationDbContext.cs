using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<GarageItem> Garages { get; }
    DbSet<GarageLookupItem> GarageLookups { get; }
    DbSet<GarageServiceItem> GarageServices { get; }
    DbSet<GarageEmployeeItem> GarageEmployees { get; }
    DbSet<GarageEmployeeWorkSchemaItem> GarageEmployeeWorkSchemaItems { get; }
    DbSet<GarageEmployeeWorkExperienceItem> GarageEmployeeWorkExperienceItems { get; }

    DbSet<VehicleLookupItem> VehicleLookups { get; }
    DbSet<VehicleServiceLogItem> VehicleServiceLogs { get; }

    DbSet<ConversationItem> Conversations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void SetQueryTrackingBehavior(QueryTrackingBehavior behavior);

}
