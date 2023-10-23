using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Whatsapp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Extensions;

namespace AutoHelper.Whatsapp;

public static class ConfigureService
{

    public static void AddWhatsappServices(this IServiceCollection services, IConfiguration configuration)
    {
        WhatsAppBusinessCloudApiConfig whatsAppConfig = new WhatsAppBusinessCloudApiConfig();
        whatsAppConfig.WhatsAppBusinessPhoneNumberId = configuration["WhatsAppBusinessCloudApiConfiguration:WhatsAppBusinessPhoneNumberId"] ?? throw new ArgumentNullException(nameof(whatsAppConfig.WhatsAppBusinessPhoneNumberId));
        whatsAppConfig.WhatsAppBusinessAccountId = configuration["WhatsAppBusinessCloudApiConfiguration:WhatsAppBusinessAccountId"] ?? throw new ArgumentNullException(nameof(whatsAppConfig.WhatsAppBusinessAccountId));
        whatsAppConfig.WhatsAppBusinessId = configuration["WhatsAppBusinessCloudApiConfiguration:WhatsAppBusinessId"] ?? throw new ArgumentNullException(nameof(whatsAppConfig.WhatsAppBusinessId));
        whatsAppConfig.AccessToken = configuration["WhatsAppBusinessCloudApiConfiguration:AccessToken"] ?? throw new ArgumentNullException(nameof(whatsAppConfig.AccessToken));

        services.AddWhatsAppBusinessCloudApiService(whatsAppConfig);
        services.AddScoped<IWhatsappService, WhatsappService>();
    }

}
