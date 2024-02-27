using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Infrastructure.Common;
using AutoHelper.Infrastructure.Persistence;
using AutoHelper.Infrastructure.Persistence.Interceptors;
using AutoHelper.Infrastructure.Services;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        //// Database and Health Checks
        //services.AddDbContextCheck<ApplicationDbContext>();

        CultureConfig.SetGlobalCultureToNL();
        services.AddTransient<AuditableEntitySaveChangesInterceptor>();

        if (bool.Parse(configuration["UseInMemoryDatabase"]!) == true)
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
                    builder =>
                    {
                        builder.CommandTimeout((int)TimeSpan.FromMinutes(25).TotalSeconds);// needed for data migrations
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        builder.UseNetTopologySuite();
                    }
                ),
                ServiceLifetime.Transient
            );
        }

        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<IDateTime, DateTimeService>();

        services.AddScoped<IRDWApiClient, RDWApiClient>();
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
        services.AddScoped<IWebScraperClient, WebScraperClient>();
        services.AddScoped<IGoogleApiClient, GoogleApiClient>();
        services.AddScoped<HtmlWeb>();


        return services;
    }

    public static void UseInfrastructureServices(this IServiceProvider services)
    {
        //using var scope = services.CreateScope();
        //var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        //initialiser.InitialiseAsync().Wait();
        //initialiser.SeedAsync().Wait();
    }

}
