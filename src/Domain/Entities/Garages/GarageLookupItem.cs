using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using AutoHelper.Domain.Entities.Vehicles;
using NetTopologySuite.Geometries;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using System.Collections;

namespace AutoHelper.Domain.Entities.Garages;

public class GarageLookupItem
{
    public GarageLookupItem()
    { }

    [Key]
    [Required]
    public string Identifier { get; set; }

    /// <summary>
    /// Provide a reference to the GarageItem that this GarageSearchableItem is associated with.
    /// When null this GarageSearchableItem is not associated with any GarageItem.
    /// The garage still do not have a GarageItem until the GarageItem is created.
    /// </summary>
    public Guid? GarageId { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    /// <summary>
    /// It is possible that the garage location is not known.
    /// </summary>
    public Geometry? Location { get; set; }

    public string? Image { get; set; } = null;
    public string? ImageThumbnail { get; set; } = null;

    public string? Status { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

    /// <summary>
    /// Contact identifier for the conversation with the garage.
    /// This can be a email address or whatsapp number.
    /// When defined, the garage is available for conversation.
    /// </summary>
    public string? ConversationContactEmail { get; set; }
    public string? ConversationContactWhatsappNumber { get; set; }

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
            DaysOfWeekString = value == null ? "" : string.Join(",", value);
        }
    }
    public string DaysOfWeekString { get; set; } = "";

    public string? Website { get; set; }

    public float? Rating { get; set; }

    public int? UserRatingsTotal { get; set; }

    public ICollection<GarageLookupServiceItem> Services { get; set; }

    [Required]
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; }

    [Required]
    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }

}
