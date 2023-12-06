using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;

namespace AutoHelper.Application.Garages._DTOs;

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
        Services = garageLookupItem.Services == null ? new List<GarageLookupServiceItem>() : garageLookupItem.Services;
        DistanceInMeter = (int)distanceInMeters;
        Rating = garageLookupItem.Rating;
        UserRatingsTotal = garageLookupItem.UserRatingsTotal;
    }

    public Guid? GarageId { get; set; }

    public string Identifier { get; set; }

    public string Name { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string? Website { get; set; }

    public int[] DaysOfWeek { get; set; }

    public ICollection<GarageLookupServiceItem> Services { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    /// <summary>
    /// Only used for filtering and ordering
    /// </summary>
    public int DistanceInMeter { get; set; }

}