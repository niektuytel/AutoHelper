using AutoHelper.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using AutoHelper.Application.Conversations._DTOs;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace AutoHelper.Messaging.Services;

internal class OutlookService : IMailingService
{
    private const string TemplateConfirmationId = "send-message-with-confirmation";
    private const string TemplateBasicId = "send_message_with_basic_content";
    private const string TemplateVehicleInfoId = "send_message_with_vehicle_information";

    private readonly IConfiguration _configuration;
    private readonly bool _isDevelopment;
    private readonly string _developEmailAddress;
    //private readonly Client _sparkPostClient;

    public OutlookService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _isDevelopment = _configuration["Environment"] == "Development";
        _developEmailAddress = _configuration["SparkPost:TestEmailAddress"]!;

        var apiKey = _configuration["SparkPost:ApiKey"]!;
        //_sparkPostClient = new Client(apiKey, "https://api.eu.sparkpost.com");
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send_message_with_basic_content/
    /// </summary>
    public async Task SendBasicMailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName, string messageContent)
    {
        throw new NotImplementedException();

        ////var model = new TestTemplate
        ////{
        ////    FirstName = "Martin",
        ////    LastName = "Whatever",
        ////    Orders = new[] {
        ////        new Templates.Order { Id = 1, Qty = 5, Price = 29.99 },
        ////        new Templates.Order { Id = 2, Qty = 1, Price = 9.99 }
        ////    }
        ////};

        ////string mailBody = Razor.Parse(template, model);





        //var transmission = new Transmission();
        //transmission.Content.TemplateId = TemplateBasicId;
        //transmission.SubstitutionData["sender_name"] = senderContactName;
        //transmission.SubstitutionData["content"] = messageContent;
        //transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        //var recipient = new Recipient
        //{
        //    Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        //};
        //transmission.Recipients.Add(recipient);

        //try
        //{
        //    var response = await _sparkPostClient.Transmissions.Send(transmission);
        //    if (response.TotalRejectedRecipients > 0)
        //    {
        //        throw new SmtpFailedRecipientException($"Failed to {nameof(SendBasicMailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    // TODO: Handle with ILogger or on hangfire
        //    Console.WriteLine(ex.Message);
        //    throw;
        //}
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send_message_with_vehicle_information
    /// </summary>
    public async Task SendVehicleRelatedEmailAsync(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalDtoItem vehicleInfo, string messageContent)
    {
        var tenantId = "24da8a0e-bbf7-4c8e-9705-5707164f1709";
        var clientId = "da79cb8a-7cab-4d49-939f-e05bd2bb9b3c";
        var clientSecret = "opf8Q~U~rDx3nOT4tri8~kzTnson347fmAOuabId";

        var scopes = new[] { "https://graph.microsoft.com/.default" }; // Use .default for client credentials flow

        var options = new TokenCredentialOptions
        {
            AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
        };

        var clientSecretCredential = new ClientSecretCredential(
            tenantId,
            clientId,
            clientSecret,
            options);

        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
        
        // Compose the message
        var message = new Message
        {
            Subject = "Enter your email subject here",
            Body = new ItemBody
            {
                ContentType = BodyType.Html,
                Content = "Your email content here. Can be HTML."
            },
            ToRecipients = new List<Microsoft.Graph.Models.Recipient>()
            {
                new Microsoft.Graph.Models.Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = "ntuijtel@gmail.com"
                    }
                }
            },
            // Add more properties as needed
        };

        // Send the email
        await graphClient.Users["contact@autohelper.com"]
            .SendMail.PostAsync(
                new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody()
                {
                    Message = message,
                }
            );
            //.SendMail(message, true) // The second parameter is 'saveToSentItems'
            //.Request()
            //.PostAsync();

        // https://developer.microsoft.com/en-us/graph/graph-explorer

        throw new NotImplementedException();
        //var client = new SmtpClient("smtp.autohelper.nl", 587)
        //{
        //    Credentials = new NetworkCredential("contact@autohelper.com", "Auto1337!"),
        //    EnableSsl = true
        //};

        //var mailMessage = new MailMessage
        //{
        //    From = new MailAddress("contact@autohelper.com"),
        //    Subject = "Subject Here",
        //    Body = "Your Email Template Here",
        //    IsBodyHtml = true
        //};
        //mailMessage.To.Add("ntuijtel@gmail.com");

        //client.Send(mailMessage);



        //var transmission = new Transmission();
        //transmission.Content.TemplateId = TemplateVehicleInfoId;
        //transmission.SubstitutionData["license_plate"] = vehicleInfo.LicensePlate;
        //transmission.SubstitutionData["content"] = messageContent;
        //transmission.SubstitutionData["feul"] = vehicleInfo.FuelType;
        //transmission.SubstitutionData["model"] = $"{vehicleInfo.Brand} {vehicleInfo.Model}({vehicleInfo.YearOfFirstAdmission})";// Dacia Sandero (2008);
        //transmission.SubstitutionData["mot"] = vehicleInfo.MOTExpiryDate;
        //transmission.SubstitutionData["nap"] = vehicleInfo.Mileage;
        //transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        //var recipient = new Recipient
        //{
        //    Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        //};
        //transmission.Recipients.Add(recipient);

        //try
        //{
        //    var response = await _sparkPostClient.Transmissions.Send(transmission);
        //    if (response.TotalRejectedRecipients > 0)
        //    {
        //        throw new SmtpFailedRecipientException($"Failed to {nameof(SendBasicMailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    // TODO: Handle with ILogger or on hangfire
        //    Console.WriteLine(ex.Message);
        //    throw;
        //}
    }

    /// <summary>
    /// https://app.eu.sparkpost.com/templates/edit/send-message-with-confirmation
    /// </summary>
    public async Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string receiverContactName)
    {
        throw new NotImplementedException();
        //var transmission = new Transmission();
        //transmission.Content.TemplateId = TemplateConfirmationId;
        //transmission.SubstitutionData["receiver_name"] = receiverContactName;
        //transmission.SubstitutionData["conversation_id"] = conversationId.ToString().Split('-')[0];

        //var recipient = new Recipient
        //{
        //    Address = new SparkPost.Address { Email = GetSecuredEmailAddress(receiverContactIdentifier) }
        //};
        //transmission.Recipients.Add(recipient);

        //try
        //{
        //    var response = await _sparkPostClient.Transmissions.Send(transmission);
        //    if (response.TotalRejectedRecipients > 0)
        //    {
        //        throw new SmtpFailedRecipientException($"Failed to {nameof(SendConfirmationEmailAsync)} email to: {receiverContactIdentifier} with conversationId:{conversationId}");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    // TODO: Handle with ILogger or on hangfire
        //    Console.WriteLine(ex.Message);
        //    throw;
        //}
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
