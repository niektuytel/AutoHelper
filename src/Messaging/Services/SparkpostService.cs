using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;

namespace AutoHelper.Messaging.Services;

internal class SparkpostService : IMailingService
{
    public Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName)
    {
        var templateId = "send-message-with-confirmation";

        // hello world
        //var client = new HttpClient();
        //var request = new HttpRequestMessage(HttpMethod.Post, "https://api.sparkpost.com/api/v1/transmissions?num_rcpt_errors=3");
        //request.Headers.Add("Accept", "application/json");
        //request.Headers.Add("Authorization", "70c81fa20ce8aabb07242251e79633675f89c054");
        //var content = new StringContent("{\n  \"campaign_id\": \"postman_template_example\",\n  \"recipients\": [\n    {\n      \"address\": \"n.tuytel@dutchgrit.nl\"\n    }\n  ],\n  \"content\": {\n    \"template_id\": \"my-first-email\"\n  }\n}\n", null, "application/json");
        //request.Content = content;
        //var response = await client.SendAsync(request);
        //response.EnsureSuccessStatusCode();
        //Console.WriteLine(await response.Content.ReadAsStringAsync());


        var subject = "AutoHelper - Je bericht is successvol gestuurd naar de garage.";
        var message = $"Hallo, We hebben je bericht verstuurd en hopen op zo snel mogelijk een antwoord te hebben.\n\n Het gaat om het bericht: \n''";

        throw new NotImplementedException();
    }

    public Task SendBasicMailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName, string subject, string messageContent)
    {
        throw new NotImplementedException();
    }

    public Task SendVehicleRelatedEmailAsync(string receiverContactIdentifier, Guid conversationId, VehicleTechnicalBriefDtoItem vehicleInfo, string subject, string messageContent)
    {
        throw new NotImplementedException();
    }
}
