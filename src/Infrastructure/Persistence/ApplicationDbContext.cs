﻿using System.Reflection;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Admin;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Messages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Infrastructure.Common.Extentions;
using AutoHelper.Infrastructure.Persistence.Interceptors;
using EFCore.BulkExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace AutoHelper.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor)
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    public DbSet<VehicleLookupItem> VehicleLookups => Set<VehicleLookupItem>();
    public DbSet<VehicleServiceLogItem> VehicleServiceLogs => Set<VehicleServiceLogItem>();
    public DbSet<VehicleTimelineItem> VehicleTimelineItems => Set<VehicleTimelineItem>();

    public DbSet<GarageItem> Garages => Set<GarageItem>();
    public DbSet<GarageServiceItem> GarageServices => Set<GarageServiceItem>();
    public DbSet<GarageLookupItem> GarageLookups => Set<GarageLookupItem>();
    public DbSet<GarageLookupServiceItem> GarageLookupServices => Set<GarageLookupServiceItem>();

    public DbSet<NotificationItem> Notifications => Set<NotificationItem>();
    public DbSet<ConversationItem> Conversations => Set<ConversationItem>();
    public DbSet<ConversationMessageItem> ConversationMessages => Set<ConversationMessageItem>();

    // Admin entities
    public DbSet<RequestLogItem> RequestLogs => Set<RequestLogItem>();

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
            .HasIndex(e => e.VehicleServiceLogId);

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
            PreserveInsertOrder = true,
            SetOutputIdentity = true,
            BatchSize = Math.Min(1000, entities.Count) // Use a smaller batch size if the list is very large
        };

        int retryCount = 0;
        int maxRetries = 5;
        TimeSpan delay = TimeSpan.FromSeconds(2);

        while (true)
        {
            try
            {
                await this.BulkInsertAsync(entities, bulkConfig: bulkConfig, cancellationToken: cancellationToken);
                break;
            }

            // 1205 is the SQL Server error code for a deadlock
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 1205 && retryCount < maxRetries)
            {
                retryCount++;
                await Task.Delay(delay, cancellationToken);
                delay = delay * 2;
            }
        }
    }

    public async Task BulkUpdateAsync<T>(IList<T> entities, CancellationToken cancellationToken) where T : class
    {
        var bulkConfig = new BulkConfig
        {
            BatchSize = entities.Count  // Optional: specify a batch size for large updates
        };

        await this.BulkUpdateAsync(entities, bulkConfig: bulkConfig, cancellationToken: cancellationToken);
    }

    public async Task BulkRemoveAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class
    {
        await this.BulkDeleteAsync(entities, cancellationToken: cancellationToken);
    }

}
