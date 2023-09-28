using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<GarageItem> Garages { get; }
    DbSet<GarageServiceItem> GarageServices { get; }
    DbSet<GarageEmployeeItem> GarageEmployees { get; }
    DbSet<GarageEmployeeWorkSchemaItem> GarageEmployeeWorkSchemaItems { get; }
    DbSet<GarageEmployeeWorkExperienceItem> GarageEmployeeWorkExperienceItems { get; }

    DbSet<VehicleItem> Vehicles { get; }
    DbSet<VehicleServiceLogItem> VehicleServiceLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

}
