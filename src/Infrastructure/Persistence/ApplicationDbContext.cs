using System.Collections.Generic;
using System.Reflection;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
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

    //public DbSet<BankingInfoItem> BankingInfos => Set<BankingInfoItem>();

    //public DbSet<GarageEmployeeItem> GarageEmployees => Set<GarageEmployeeItem>();

    //public DbSet<GarageServiceItem> GarageServices => Set<GarageServiceItem>();

    //public DbSet<LocationItem> Locations => Set<LocationItem>();

    //public DbSet<BusinessOwnerItem> BusinessOwners => Set<BusinessOwnerItem>();

    //public DbSet<VehicleItem> Vehicles => Set<VehicleItem>();

    //public DbSet<MaintenanceItem> Maintenances => Set<MaintenanceItem>();

    //public DbSet<ContactItem> Contacts => Set<ContactItem>();

    //public DbSet<VehicleOwnerItem> VehicleOwners => Set<VehicleOwnerItem>();

    //public DbSet<OrderItem> Orders => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
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

    //// Deprecated
    //public DbSet<TodoList> TodoLists => Set<TodoList>();

    //public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
