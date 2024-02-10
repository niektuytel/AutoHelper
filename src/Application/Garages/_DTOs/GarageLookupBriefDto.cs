using AutoHelper.Domain.Entities.Garages;
using AutoMapper;

namespace AutoHelper.Application.Garages._DTOs;

public class GarageLookupBriefDto
{
    public GarageLookupBriefDto(GarageLookupItem garageLookupItem, double distanceInMeters, IMapper _mapper)
    {
        GarageId = garageLookupItem.GarageId;
        Identifier = garageLookupItem.Identifier;
        Name = garageLookupItem.Name;
        ImageThumbnail = garageLookupItem.ImageThumbnail;
        Address = garageLookupItem.Address;
        City = garageLookupItem.City;
        Website = garageLookupItem.Website;
        DaysOfWeek = garageLookupItem.DaysOfWeek == null ? new string[0] : garageLookupItem.DaysOfWeek;
        Services = _mapper.Map<IEnumerable<GarageServiceDtoItem>>(garageLookupItem.Services) ?? new List<GarageServiceDtoItem>();
        Rating = garageLookupItem.Rating;
        UserRatingsTotal = garageLookupItem.UserRatingsTotal;
        DistanceInMeter = (int)distanceInMeters;
    }

    public Guid? GarageId { get; set; }

    public string Identifier { get; set; }

    public string Name { get; set; }

    public string? ImageThumbnail { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string? Website { get; set; }

    public string[] DaysOfWeek { get; set; }

    public IEnumerable<GarageServiceDtoItem> Services { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    /// <summary>
    /// Only used for filtering and ordering
    /// </summary>
    public int DistanceInMeter { get; set; }

}