using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGaragesBySearch;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Identity;
using AutoHelper.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Options;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutoHelper.Infrastructure.Persistence;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IOptions<OperationalStoreOptions> operationalStoreOptions,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) 
        : base(options, operationalStoreOptions)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<GarageItem> Garages => Set<GarageItem>();
    public DbSet<GarageServiceItem> GarageServices => Set<GarageServiceItem>();
    public DbSet<GarageEmployeeItem> GarageEmployees => Set<GarageEmployeeItem>();
    public DbSet<GarageEmployeeWorkSchemaItem> GarageEmployeeWorkSchemaItems => Set<GarageEmployeeWorkSchemaItem>();
    public DbSet<GarageEmployeeWorkExperienceItem> GarageEmployeeWorkExperienceItems => Set<GarageEmployeeWorkExperienceItem>();

    public DbSet<VehicleItem> Vehicles => Set<VehicleItem>();
    public DbSet<VehicleServiceLogItem> VehicleServiceLogs => Set<VehicleServiceLogItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        builder.Entity<GarageServiceItem>()
               .Property(p => p.Price)
               .HasColumnType("decimal(18,2)");

        builder.Entity<GarageServicesSettingsItem>()
               .Property(p => p.DeliveryPrice)
               .HasColumnType("decimal(18,2)");

        // Configuring the relationship between GarageItem and GarageEmployeeItem
        builder.Entity<GarageEmployeeItem>()
            .HasOne(e => e.Garage)
            .WithMany(g => g.Employees)
            .HasForeignKey(e => e.GarageId);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEvents(this);

        return await base.SaveChangesAsync(cancellationToken);
    }
    
}
