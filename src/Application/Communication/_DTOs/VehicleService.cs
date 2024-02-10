namespace AutoHelper.Application.Messages._DTOs;

public class VehicleService
{
    public Guid GarageServiceId { get; set; }
    public string? GarageServiceTitle { get; set; }

    public string? RelatedGarageLookupIdentifier { get; set; }
    public string? RelatedGarageLookupName { get; set; }

    public string? ConversationEmailAddress { get; set; }
    public string? ConversationWhatsappNumber { get; set; }

    public string? VehicleLicensePlate { get; set; }
    public string? VehicleLongitude { get; set; }
    public string? VehicleLatitude { get; set; }

}