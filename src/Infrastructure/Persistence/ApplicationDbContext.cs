using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Garages.Unused;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Identity;
using AutoHelper.Infrastructure.Persistence.Interceptors;
using Duende.IdentityServer.EntityFramework.Options;
using EFCore.BulkExtensions;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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

    public DbSet<VehicleLookupItem> VehicleLookups => Set<VehicleLookupItem>();
    public DbSet<VehicleServiceLogItem> VehicleServiceLogs => Set<VehicleServiceLogItem>();
    public DbSet<VehicleTimelineItem> VehicleTimelineItems => Set<VehicleTimelineItem>();

    public DbSet<GarageLookupItem> GarageLookups => Set<GarageLookupItem>();
    public DbSet<GarageItem> Garages => Set<GarageItem>();
    public DbSet<GarageServiceItem> GarageServices => Set<GarageServiceItem>();

    public DbSet<ConversationItem> Conversations => Set<ConversationItem>();
    public DbSet<ConversationMessageItem> ConversationMessages => Set<ConversationMessageItem>();

#if DEBUG
    public DbSet<GarageEmployeeItem> GarageEmployees => Set<GarageEmployeeItem>();
    public DbSet<GarageEmployeeWorkSchemaItem> GarageEmployeeWorkSchemaItems => Set<GarageEmployeeWorkSchemaItem>();
    public DbSet<GarageEmployeeWorkExperienceItem> GarageEmployeeWorkExperienceItems => Set<GarageEmployeeWorkExperienceItem>();
#endif

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);

        builder.Entity<VehicleLookupItem>()
            .HasIndex(v => v.LicensePlate)
            .IsUnique();

        builder.Entity<VehicleTimelineItem>()
            .HasIndex(e => e.VehicleLicensePlate);

        builder.Entity<VehicleTimelineItem>()
            .HasOne(e => e.VehicleLookup)
            .WithMany(g => g.Timeline)
            .HasForeignKey(e => e.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<VehicleServiceLogItem>()
            .HasIndex(e => e.VehicleLicensePlate);

        builder.Entity<VehicleServiceLogItem>()
            .HasOne(e => e.VehicleLookup)
            .WithMany(g => g.ServiceLogs)
            .HasForeignKey(e => e.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<GarageServiceItem>()
            .HasOne(e => e.Garage)
            .WithMany(g => g.Services)
            .HasForeignKey(e => e.GarageId);

        builder.Entity<ConversationItem>()
            .HasOne(e => e.RelatedVehicleLookup)
            .WithMany(g => g.Conversations)
            .HasForeignKey(e => e.VehicleLicensePlate)
            .OnDelete(DeleteBehavior.NoAction);

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

    public void SetQueryTrackingBehavior(QueryTrackingBehavior behavior)
    {
        this.ChangeTracker.QueryTrackingBehavior = behavior;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return this.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class
    {
        var bulkConfig = new BulkConfig
        {
            PreserveInsertOrder = true, // Set to true if the insert order should be preserved
            SetOutputIdentity = true,   // Set to true to update entities' identities after inserting them
            BatchSize = entities.Count  // Optional: specify a batch size for large inserts
        };

        await this.BulkInsertAsync(entities, bulkConfig: bulkConfig, cancellationToken: cancellationToken);
    }

    public async Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class
    {
        var bulkConfig = new BulkConfig
        {
            BatchSize = entities.Count  // Optional: specify a batch size for large updates
        };

        await this.BulkUpdateAsync(entities, bulkConfig: bulkConfig, cancellationToken: cancellationToken);
    }

}
