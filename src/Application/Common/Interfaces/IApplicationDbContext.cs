using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
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
    DbSet<GarageServiceItem> GarageServices { get; }
    DbSet<GarageLookupItem> GarageLookups { get; }
    DbSet<GarageLookupServiceItem> GarageLookupServices { get; }

    DbSet<ConversationItem> Conversations { get; }
    DbSet<ConversationMessageItem> ConversationMessages { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class;
    Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class;
    Task BulkRemoveAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    void SetQueryTrackingBehavior(QueryTrackingBehavior behavior);

}
