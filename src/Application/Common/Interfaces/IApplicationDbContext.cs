using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<GarageItem> Garages { get; }
    DbSet<GarageServiceItem> GarageServices { get; }
    DbSet<GarageEmployeeItem> GarageEmployees { get; }
    DbSet<GarageEmployeeWorkSchemaItem> GarageEmployeeWorkSchemaItems { get; }
    DbSet<GarageEmployeeWorkExperienceItem> GarageEmployeeWorkExperienceItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
