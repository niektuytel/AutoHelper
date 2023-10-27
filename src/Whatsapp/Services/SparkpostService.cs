using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;

namespace AutoHelper.Messaging.Services;

internal class SparkpostService : IMailingService
{
    public Task SendConfirmationEmailAsync(string receiverContactIdentifier, Guid conversationId, string senderContactName)
    {
        var templateId = "send-message-with-confirmation";




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
