using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Extensions;
using AutoHelper.Messaging.Models;
using AutoHelper.Messaging.Helpers;

namespace AutoHelper.Messaging;

public static class ConfigureService
{

    public static void AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        var whatsAppBusinessApiConfiguration = new WhatsAppBusinessCloudApiConfig();
        whatsAppBusinessApiConfiguration.WhatsAppBusinessPhoneNumberId = configuration["WhatsApp:WhatsAppBusinessPhoneNumberId"]
            ?? throw new ArgumentNullException(nameof(whatsAppBusinessApiConfiguration.WhatsAppBusinessPhoneNumberId));
        whatsAppBusinessApiConfiguration.WhatsAppBusinessAccountId = configuration["WhatsApp:WhatsAppBusinessAccountId"]
            ?? throw new ArgumentNullException(nameof(whatsAppBusinessApiConfiguration.WhatsAppBusinessAccountId));
        whatsAppBusinessApiConfiguration.WhatsAppBusinessId = configuration["WhatsApp:WhatsAppBusinessId"]
            ?? throw new ArgumentNullException(nameof(whatsAppBusinessApiConfiguration.WhatsAppBusinessId));
        whatsAppBusinessApiConfiguration.AccessToken = configuration["WhatsApp:AccessToken"]
            ?? throw new ArgumentNullException(nameof(whatsAppBusinessApiConfiguration.AccessToken));

        services.AddWhatsAppBusinessCloudApiService(whatsAppBusinessApiConfiguration);
        services.AddScoped<IWhatsappTemplateService, WhatsappTemplateService>();
        services.AddScoped<IWhatsappResponseService, WhatsappResponseService>();
        services.AddScoped<IIdentification, PhoneNumberHelper>();
        services.AddScoped<IMailingService, GraphEmailService>();

    }

}
