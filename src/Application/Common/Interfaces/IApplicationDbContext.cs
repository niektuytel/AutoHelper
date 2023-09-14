using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<GarageItem> Garages { get; }
    DbSet<GarageServiceItem> GarageServices { get; }
    DbSet<GarageEmployeeItem> GarageEmployees { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
