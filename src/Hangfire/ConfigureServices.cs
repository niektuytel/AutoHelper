using System.Globalization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Application.Vehicles.Commands;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Hangfire.Persistence;
using AutoHelper.Hangfire.Services;
using Hangfire;
using Hangfire.Console;
using Hangfire.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using Microsoft.Extensions.Hosting;
using Hangfire.Dashboard;
using System.Net;
using Hangfire.Common;
using Hangfire.Dashboard.Management.v2.Support;
using Hangfire.Dashboard.Management.v2;
using Hangfire.SqlServer;

namespace AutoHelper.Hangfire;

public static class ConfigureServices
{
    public static void AddHangfireServices(this WebApplicationBuilder builder)
    {
        var hangfireConnection = builder.Configuration.GetConnectionString("HangfireConnection");
        builder.Services.AddDbContext<HangfireDbContext>(o => o.UseSqlServer(hangfireConnection));

        builder.Services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions
            {
                InvisibilityTimeout = TimeSpan.FromDays(1)//,
                //SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                //QueuePollInterval = TimeSpan.Zero,
                //UseRecommendedIsolationLevel = true,
                //DisableGlobalLocks = true
            })
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseConsole()
            .UseMediatR();

            config.UseManagementPages(typeof(ConfigureServices).Assembly);
        });

        builder.Services.AddTransient<IQueueService, HangfireJobService>();

        // production we use the dashboard web service to run the jobs
        if (builder.Environment.IsDevelopment())
        {
            builder.AddHangfireServerInstance();
        }
    }

    public static void AddHangfireServerInstance(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfireServer(options =>
        {
            var queues = new List<string>
            {
                "default",
                "critical",
                "long-running",
                nameof(UpsertGarageLookupsCommand).ToLower(),
                nameof(UpsertVehicleLookupsCommand).ToLower(),
                nameof(StartConversationCommand).ToLower(),
            };
            queues.AddRange(JobsHelper.GetAllQueues());

            //options.HeartbeatInterval = TimeSpan.FromSeconds(5);
            options.CancellationCheckInterval = TimeSpan.FromSeconds(5);
            options.WorkerCount = Environment.ProcessorCount * 5;
            options.Queues = queues.Distinct().ToArray();
        });
    }

    public static void UseMediatR(this IGlobalConfiguration configuration)
    {
        var jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        configuration.UseSerializerSettings(jsonSettings);
    }

    public static void UseHangfireServices(this WebApplication app, IServiceScope scope)
    {
        //// define that we want to use batches
        //GlobalConfiguration.Configuration.UseBatches();

        if(app.Environment.IsDevelopment())
        {
            // Migrate and Update the database
            var context = scope.ServiceProvider.GetRequiredService<HangfireDbContext>();
            var created = context.Database.EnsureCreated();
            context.Database.Migrate();
        }
        
        // production we use the dashboard web service to run the dashboard
        if (app.Environment.IsDevelopment())
        {
            app.UseHangfireDashboardInstance();
        }
    }

    public static void UseHangfireDashboardInstance(this WebApplication app)
    {
        app.UseHangfireDashboard("", new DashboardOptions()
        {
            Authorization = new[] { new HangfireDashboardAuthFilter(app.Environment) },
            DisplayStorageConnectionString = false,
            DashboardTitle = "Hangfire Management",
            StatsPollingInterval = 5000
        });
    }
}
