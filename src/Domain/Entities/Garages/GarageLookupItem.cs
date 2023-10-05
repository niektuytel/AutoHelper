using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using AutoHelper.Domain.Entities.Vehicles;
using AutoHelper.Domain.Entities.Deprecated;

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

    [Required]
    public string Status { get; set; }

    public string? ImageUrl { get; set; }

    [NotMapped]
    public int[]? DaysOfWeek
    {
        get
        {
            if (_daysOfWeek == null)
            {
                return new int[0];
            }
            return Array.ConvertAll(_daysOfWeek.Split(','), int.Parse);
        }
        set
        {
            _daysOfWeek = value == null? "" : string.Join(",", value);
        }
    }
    private string _daysOfWeek = "";

    public string? PhoneNumber { get; set; }//TODO

    public string? WhatsappNumber { get; set; }//TODO

    public string? EmailAddress { get; set; }//TODO

    public string? Website { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public float Longitude { get; set; }

    [Required]
    public float Latitude { get; set; }

    public bool HasPickupService { get; set; } = false;

    public bool HasReplacementTransportService { get; set; } = false;

    public bool HasBestPrice { get; set; } = false;

    public string GoogleApiDetailsJson { get; set; }

}
