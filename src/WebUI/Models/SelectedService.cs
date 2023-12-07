using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.WebUI.Models;

public class SelectedService
{
    public Guid GarageServiceId { get; set; }

    public string RelatedGarageLookupIdentifier { get; set; }

    public string RelatedGarageLookupName { get; set; }

    public string ConversationContactEmail { get; set; }

    public string ConversationContactWhatsappNumber { get; set; }

    public string VehicleLicensePlate { get; set; }

    public string VehicleLongitude { get; set; }

    public string VehicleLatitude { get; set; }

    public string? GarageContactIdentifier { get; set; }

    public ContactType? GarageContactType { get; set; }

}