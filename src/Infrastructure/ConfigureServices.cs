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
        CultureConfig.SetGlobalCultureToNL();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

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
                )
            );
        }

        services.AddTransient<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddTransient<IDateTime, DateTimeService>();

        services.AddTransient<IRDWApiClient, RDWApiClient>();
        services.AddTransient<IBlobStorageService, AzureBlobStorageService>();
        services.AddTransient<IWebScraperClient, WebScraperClient>();
        services.AddTransient<IGoogleApiClient, GoogleApiClient>();
        services.AddTransient<HtmlWeb>();


        return services;
    }
}
