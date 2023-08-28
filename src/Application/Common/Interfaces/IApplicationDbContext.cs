using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<GarageItem> Garages { get; }
    //DbSet<BankingInfoItem> BankingInfos { get; }
    //DbSet<GarageEmployeeItem> GarageEmployees { get; }
    //DbSet<GarageServiceItem> GarageServices { get; }
    //DbSet<LocationItem> Locations { get; }
    //DbSet<BusinessOwnerItem> BusinessOwners { get; }
    //DbSet<VehicleItem> Vehicles { get; }
    //DbSet<MaintenanceItem> Maintenances { get; }
    //DbSet<ContactItem> Contacts { get; }
    //DbSet<VehicleOwnerItem> VehicleOwners { get; }
    //DbSet<OrderItem> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
