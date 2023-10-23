
using AutoHelper.Application.Common.Interfaces;
using Mailing.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoHelper.Mailing;

public static class ConfigureServices
{
    public static void AddMailingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMailingService, CustomMailingService>();
    }

}
