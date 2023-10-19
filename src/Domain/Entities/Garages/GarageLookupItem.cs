using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using AutoHelper.Domain.Entities.Vehicles;
using NetTopologySuite.Geometries;
using AutoHelper.Domain.Entities.Conversations;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageLookupItem : BaseEntity
{

    /// <summary>
    /// Provide a reference to the GarageItem that this GarageSearchableItem is associated with.
    /// When null this GarageSearchableItem is not associated with any GarageItem.
    /// The garage still do not have a GarageItem until the GarageItem is created.
    /// </summary>
    public Guid? GarageId { get; set; }

    [Required]
    public string Identifier { get; set; }

    [Required]
    public string Name { get; set; }

    [NotMapped]
    public GarageServiceType[] KnownServices
    {
        get
        {
            if (KnownServicesString == null)
            {
                return new GarageServiceType[0];
            }
            return KnownServicesString
                .Split(';')
                .Select(x => (GarageServiceType)int.Parse(x))
                .ToArray();
        }
        set
        {
            KnownServicesString = value == null ? "" : string.Join(";", value.Select(v => ((int)v).ToString()));
        }
    }

    [Required]
    public string KnownServicesString { get; set; } = "";

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    public Geometry? Location { get; set; }

    public string? Status { get; set; }

    [NotMapped]
    public int[]? DaysOfWeek
    {
        get
        {
            if (string.IsNullOrEmpty(DaysOfWeekString))
            {
                return new int[0];
            }
            return Array.ConvertAll(DaysOfWeekString.Split(','), int.Parse);
        }
        set
        {
            DaysOfWeekString = value == null? "" : string.Join(",", value);
        }
    }
    public string DaysOfWeekString { get; set; } = "";

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    public string? Website { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    public bool HasPickupService { get; set; } = false;// TODO: Implement pickup service

    public bool HasReplacementTransportService { get; set; } = false;// TODO: Implement replacement transport service

    public GarageLookupLargeItem? LargeData { get; set; } = null;

    [Required]
    public ICollection<ConversationItem> Conversations { get; set; } = new List<ConversationItem>();

}
