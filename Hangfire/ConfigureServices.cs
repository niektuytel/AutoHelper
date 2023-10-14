using System.Globalization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Hangfire.Persistence;
using AutoHelper.Hangfire.Services;
using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AutoHelper.Hangfire;

public static class ConfigureServices
{
    public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConnection = configuration.GetConnectionString("HangfireConnection");
        services.AddDbContext<HangfireDbContext>(o => o.UseSqlServer(hangfireConnection));
        services.AddHangfire(configuration =>
        {
            configuration.UseSqlServerStorage(hangfireConnection);
            configuration.UseMediatR();
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(hangfireConnection);
        });
        services.AddHangfireConsoleExtensions();
        services.AddHangfireServer();
        services.AddTransient<IQueueingJobService, HangfireJobService>();
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

        // Migrate and Update the database
        var context = scope.ServiceProvider.GetRequiredService<HangfireDbContext>();
        var created = context.Database.EnsureCreated();
        context.Database.Migrate();

        app.UseHangfireDashboard();
    }

}
