using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Garages.Unused;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<VehicleLookupItem> VehicleLookups { get; }
    DbSet<VehicleServiceLogItem> VehicleServiceLogs { get; }
    DbSet<VehicleTimelineItem> VehicleTimelineItems { get; }

    DbSet<GarageItem> Garages { get; }
    DbSet<GarageLookupItem> GarageLookups { get; }
    DbSet<GarageServiceItem> GarageServices { get; }

    DbSet<ConversationItem> Conversations { get; }
    DbSet<ConversationMessageItem> ConversationMessages { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class;
    Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void SetQueryTrackingBehavior(QueryTrackingBehavior behavior);

}
