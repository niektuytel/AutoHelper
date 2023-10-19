using System.Globalization;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Identity;
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
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        builder.UseNetTopologySuite();
                    }
                )
            );
        }

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services
            .AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Require that usernames are email format
                options.User.RequireUniqueEmail = true;

                // Set a specific allowed username character set (e.g., alphanumeric)
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        services.AddAuthentication()
            .AddIdentityServerJwt();

        //services.AddAuthorization(options =>
        //    options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator")));

        services.AddTransient<HtmlWeb>();
        services.AddTransient<WebScraperClient>();
        services.AddTransient<RDWApiClient>();
        services.AddTransient<GoogleApiClient>();
        services.AddTransient<IVehicleInfoService, VehicleInfoService>();
        services.AddTransient<IGarageInfoService, GarageInfoService>();

        return services;
    }
}
