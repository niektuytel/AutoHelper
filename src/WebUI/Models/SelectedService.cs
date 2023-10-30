using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.WebUI.Models;

public class SelectedService
{
    public Guid RelatedGarageLookupId { get; set; }

    public GarageServiceType RelatedServiceType { get; set; }

    public string VehicleLicensePlate { get; set; }

    public string VehicleLongitude { get; set; }

    public string VehicleLatitude { get; set; }

    public string? ReceiverWhatsAppNumberOrEmail { get; set; }

}