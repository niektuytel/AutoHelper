using AutoHelper.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using SparkPost;
using WhatsappBusiness.CloudApi.Exceptions;
using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Webhook;
using System.Net.Mail;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Messaging.Templates;
using RazorEngine;

namespace AutoHelper.Messaging.Services;

internal class OutlookService : IMailingService
{
    private const string TemplateConfirmationId = "send-message-with-confirmation";
    private const string TemplateBasicId = "send_message_with_basic_content";
    private const string TemplateVehicleInfoId = "send_message_with_vehicle_information";

    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developEmailAddress;
    private readonly Client _sparkPostClient;

    public OutlookService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isDevelopment = _configuration["Environment"] == "Development";
        _developEmailAddress = _configuration["SparkPost:TestEmailAddress"]!;

        var apiKey = _configuration["SparkPost:ApiKey"]!;
        _sparkPostClient = new Client(apiKey, "https://api.eu.sparkpost.com");
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send_message_with_basic_content/
    /// </summary>
    public async Task SendBasicMailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName, string messageContent)
    {

        //var model = new TestTemplate
        //{
        //    FirstName = "Martin",
        //    LastName = "Whatever",
        //    Orders = new[] {
        //        new Templates.Order { Id = 1, Qty = 5, Price = 29.99 },
        //        new Templates.Order { Id = 2, Qty = 1, Price = 9.99 }
        //    }
        //};

        //string mailBody = Razor.Parse(template, model);





        var transmission = new Transmission();
        transmission.Content.TemplateId = TemplateBasicId;
        transmission.SubstitutionData["sender_name"] = senderContactName;
        transmission.SubstitutionData["content"] = messageContent;
        transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        var recipient = new Recipient
        {
            Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        };
        transmission.Recipients.Add(recipient);

        try
        {
            var response = await _sparkPostClient.Transmissions.Send(transmission);
            if (response.TotalRejectedRecipients > 0)
            {
                throw new SmtpFailedRecipientException($"Failed to {nameof(SendBasicMailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
            }
        }
        catch (Exception ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send_message_with_vehicle_information
    /// </summary>
    public async Task SendVehicleRelatedEmailAsync(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicleInfo, string messageContent)
    {
        var transmission = new Transmission();
        transmission.Content.TemplateId = TemplateVehicleInfoId;
        transmission.SubstitutionData["license_plate"] = vehicleInfo.LicensePlate;
        transmission.SubstitutionData["content"] = messageContent;
        transmission.SubstitutionData["feul"] = vehicleInfo.FuelType;
        transmission.SubstitutionData["model"] = $"{vehicleInfo.Brand} {vehicleInfo.Model}({vehicleInfo.YearOfFirstAdmission})";// Dacia Sandero (2008);
        transmission.SubstitutionData["mot"] = vehicleInfo.MOTExpiryDate;
        transmission.SubstitutionData["nap"] = vehicleInfo.Mileage;
        transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        var recipient = new Recipient
        {
            Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        };
        transmission.Recipients.Add(recipient);

        try
        {
            var response = await _sparkPostClient.Transmissions.Send(transmission);
            if (response.TotalRejectedRecipients > 0)
            {
                throw new SmtpFailedRecipientException($"Failed to {nameof(SendBasicMailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
            }
        }
        catch (Exception ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send-message-with-confirmation
    /// </summary>
    public async Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string receiverContactName)
    {
        var transmission = new Transmission();
        transmission.Content.TemplateId = TemplateConfirmationId;
        transmission.SubstitutionData["receiver_name"] = receiverContactName;
        transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        var recipient = new Recipient
        {
            Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        };
        transmission.Recipients.Add(recipient);

        try
        {
            var response = await _sparkPostClient.Transmissions.Send(transmission);
            if (response.TotalRejectedRecipients > 0)
            {
                throw new SmtpFailedRecipientException($"Failed to {nameof(SendConfirmationEmailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
            }
        }
        catch (Exception ex)
        {
            // TODO: Handle with ILogger or on hangfire
            Console.WriteLine(ex.Message);
            throw;
        }
    }

    private string GetSecuredEmailAddress(string address)
    {
        if (_isDevelopment)
        {
            return _developEmailAddress;
        }

        return address;
    }

}
