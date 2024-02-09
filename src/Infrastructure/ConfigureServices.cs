using System.Globalization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Infrastructure.Persistence.Interceptors;
using AutoHelper.Infrastructure.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        CultureConfig.SetGlobalCultureToNL();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("AutoHelperDb");
                //options.UseNetTopologySuite();
            });
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    builder => {
                        builder.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds);// needed for data migrations
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        builder.UseNetTopologySuite();
                    }
                )
            );
        }

        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddTransient<RDWApiClient>();
        services.AddTransient<IVehicleService, VehicleService>();
        services.AddTransient<IVehicleTimelineService, VehicleTimelineService>();
        services.AddTransient<IGarageService, GarageService>();
        services.AddTransient<IBlobStorageService, AzureBlobStorageService>();
        services.AddTransient<HtmlWeb>();
        services.AddTransient<WebScraperClient>();
        services.AddTransient<GoogleApiClient>();


        return services;
    }
}
