using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;

namespace WebUI.Models;

public class StartConversationBody
{
    public Guid RelatedGarageLookupId { get; set; }

    public GarageServiceType[] RelatedServiceTypes { get; set; }

    public string VehicleLicensePlate { get; set; }

    public string VehicleLongitude { get; set; }

    public string VehicleLatitude { get; set; }

    public string? VehiclePhoneNumber { get; set; }

    public string? VehicleWhatsappNumber { get; set; }

    public string? VehicleEmailAddress { get; set; }

    public string? SenderWhatsAppNumberOrEmail { get; set; }

    public string? ReceiverWhatsAppNumberOrEmail { get; set; }

    public ConversationMessageType MessageType { get; set; }

    public string MessageContent { get; set; }
}