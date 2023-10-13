using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;

namespace AutoHelper.Application.Garages.Queries.GetGaragesLookups;

public class GarageLookupBriefDto
{
    public GarageLookupBriefDto(GarageLookupItem garageLookupItem, double distanceInMeters)
    {
        GarageId = garageLookupItem.GarageId;
        Identifier = garageLookupItem.Identifier;
        Name = garageLookupItem.Name;
        Address = garageLookupItem.Address;
        City = garageLookupItem.City;
        Website = garageLookupItem.Website;
        DaysOfWeek = garageLookupItem.DaysOfWeek == null ? new int[0] : garageLookupItem.DaysOfWeek;
        KnownServices = garageLookupItem.KnownServices == null ? new GarageServiceType[0] : garageLookupItem.KnownServices;
        DistanceInMeter = (int)distanceInMeters;
        Rating = garageLookupItem.Rating;
        UserRatingsTotal = garageLookupItem.UserRatingsTotal;
        HasPickupService = garageLookupItem.HasPickupService;
        HasReplacementTransportService = garageLookupItem.HasReplacementTransportService;
    }

    public Guid? GarageId { get; set; }

    public string Identifier { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string? Website { get; set; }

    public int[] DaysOfWeek { get; set; }

    public GarageServiceType[] KnownServices { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    /// <summary>
    /// Only used for filtering and ordering
    /// </summary>
    public int DistanceInMeter { get; set; }

    public bool HasPickupService { get; set; } = false;

    public bool HasReplacementTransportService { get; set; } = false;

}