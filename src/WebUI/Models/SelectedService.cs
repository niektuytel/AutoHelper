using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.WebUI.Models;

public class SelectedService
{
    public Guid RelatedGarageLookupId { get; set; }

    public string RelatedGarageLookupIdentifier { get; set; }

    public string RelatedGarageLookupName { get; set; }

    public GarageServiceType RelatedServiceType { get; set; }
    public string RelatedServiceTypeTitle { get; set; }

    public string VehicleLicensePlate { get; set; }

    public string VehicleLongitude { get; set; }

    public string VehicleLatitude { get; set; }

    public string? ReceiverWhatsAppNumberOrEmail { get; set; }

}