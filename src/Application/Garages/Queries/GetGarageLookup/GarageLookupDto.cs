using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Garages;
using NetTopologySuite.Geometries;
using NetTopologySuite.Index.HPRtree;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookup;

public class GarageLookupDto: IMapFrom<GarageLookupItem>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Provide a reference to the GarageItem that this GarageSearchableItem is associated with.
    /// When null this GarageSearchableItem is not associated with any GarageItem.
    /// The garage still do not have a GarageItem until the GarageItem is created.
    /// </summary>
    public Guid? GarageId { get; set; }

    public string Identifier { get; set; }

    public string Name { get; set; }

    public string Image { get; set; }

    public string ImageThumbnail { get; set; }

    public GarageServiceType[] KnownServices { get; set; }

    public int[] DaysOfWeek { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? Website { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public bool HasPickupService { get; set; } = false;// TODO: Implement pickup service

    public bool HasReplacementTransportService { get; set; } = false;// TODO: Implement replacement transport service

}