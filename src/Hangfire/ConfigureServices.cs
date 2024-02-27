using AutoHelper.Application.Common.Interfaces.Queue;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using AutoHelper.Application.Messages.Commands.SendNotificationMessage;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;
using AutoHelper.Hangfire.Persistence;
using AutoHelper.Hangfire.Services;
using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace AutoHelper.Hangfire;

public static class ConfigureServices
{
    public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration, bool inDevelopment)
    {
        var hangfireConnection = configuration.GetConnectionString("HangfireConnection");
        services.AddDbContext<HangfireDbContext>(o => o.UseSqlServer(hangfireConnection));

        services.AddHangfire(config =>
        {
            config.UseSqlServerStorage(hangfireConnection, new SqlServerStorageOptions
            {
                InvisibilityTimeout = TimeSpan.FromDays(1)
            })
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseConsole()
            .UseMediatR();

        });

        services.AddScoped<IQueueContext, HangfireJobContext>();
        services.AddScoped<IQueueService, HangfireJobService>();

        // production we use the dashboard web service to run the jobs
        if (inDevelopment)
        {
            services.AddHangfireServerInstance();
        }
    }

    public static void AddHangfireServerInstance(this IServiceCollection services)
    {
        services.AddHangfireServer(options =>
        {
            var queues = new List<string>
            {
                "default",
                "critical",
                "long-running",
                nameof(SyncGarageLookupsCommand).ToLower(),
                nameof(SyncVehicleLookupsCommand).ToLower(),

                nameof(SendConversationMessageCommand).ToLower(),
                nameof(SendNotificationMessageCommand).ToLower(),
            };

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

    public static void UseHangfireServices(this WebApplication app)
    {
        //// define that we want to use batches
        //GlobalConfiguration.Configuration.UseBatches();

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<HangfireDbContext>();
        if (context.Database.EnsureCreated())
        {
            context.Database.Migrate();
        }

        //// production we use the dashboard web service to run the dashboard
        //if (app.Environment.IsDevelopment())
        //{
        //}

        app.UseHangfireDashboardInstance();
    }

    public static void UseHangfireDashboardInstance(this WebApplication app, string matchPath = "/hangfire")
    {
        app.UseHangfireDashboard(matchPath, new DashboardOptions()
        {
#if !DEBUG
            Authorization = new[] { new BasicAuthAuthorizationFilter(
                new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = false,
                    SslRedirect = false,
                    LoginCaseSensitive = true,
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser{ 
                            Login = app.Configuration["Hangfire:Login"], 
                            PasswordClear = app.Configuration["Hangfire:PasswordClear"] 
                        }
                    }

                }) 
            }
#endif
        });
    }
}
