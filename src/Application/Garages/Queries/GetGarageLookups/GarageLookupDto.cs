using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public class GarageLookupDto: GarageLookupItem
{
    public GarageLookupDto(GarageLookupItem garageLookupItem)
    {
        Id = garageLookupItem.Id;
        GarageId = garageLookupItem.GarageId;
        Name = garageLookupItem.Name;
        PhoneNumber = garageLookupItem.PhoneNumber;
        WhatsappNumber = garageLookupItem.WhatsappNumber;
        EmailAddress = garageLookupItem.EmailAddress;
        Address = garageLookupItem.Address;
        City = garageLookupItem.City;
        Latitude = garageLookupItem.Latitude;
        Longitude = garageLookupItem.Longitude;

        HasPickupService = true;// TODO: Implement pickup service
        HasReplacementTransportService = true;// TODO: Implement replacement transport service
        HasBestPrice = true;// TODO: Implement best price

        //HasPickupService = garageLookupItem.HasPickupService;
        //HasReplacementTransportService = garageLookupItem.HasReplacementTransportService;
        //HasBestPrice = garageLookupItem.HasBestPrice;
    }

    /// <summary>
    /// Only used for filtering and ordering
    /// </summary>
    public int DistanceInKm { get; set; }

}