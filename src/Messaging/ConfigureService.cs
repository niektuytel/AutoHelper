using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Interfaces.Messaging;
using AutoHelper.Application.Common.Interfaces.Messaging.Email;
using AutoHelper.Application.Common.Interfaces.Messaging.Whatsapp;
using AutoHelper.Messaging.Helpers;
using AutoHelper.Messaging.Interfaces;
using AutoHelper.Messaging.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Extensions;

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

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailNotificationService, EmailNotificationService>();
        services.AddScoped<IEmailConversationService, EmailConversationService>();

        services.AddScoped<IWhatsappService, WhatsappService>();
        services.AddScoped<IWhatsappNotificationService, WhatsappNotificationService>();
        services.AddScoped<IWhatsappConversationService, WhatsappConversationService>(); 

        services.AddScoped<IIdentificationHelper, IdentificationHelper>();
        services.AddScoped<IWhatsappResponseService, WhatsappResponseService>();
    }

}
