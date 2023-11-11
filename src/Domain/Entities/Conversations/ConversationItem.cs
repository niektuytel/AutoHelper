using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationItem : BaseAuditableEntity
{
    public PriorityLevel Priority { get; set; } = PriorityLevel.Low;

    [Required]
    public string VehicleLicensePlate { get; set; }

    [ForeignKey(nameof(VehicleLicensePlate))]
    public VehicleLookupItem RelatedVehicleLookup { get; set; } = null!;

    [Required]
    public Guid RelatedGarageLookupId { get; set; }
    
    [ForeignKey(nameof(RelatedGarageLookupId))]
    public GarageLookupItem RelatedGarageLookup { get; set; } = null!;

    [NotMapped]
    public GarageServiceType[] RelatedServiceTypes
    {
        get
        {
            if (RelatedServiceTypesString == null)
            {
                return new GarageServiceType[0];
            }
            return RelatedServiceTypesString
                .Split(';')
                .Select(x => (GarageServiceType)int.Parse(x))
                .ToArray();
        }
        set
        {
            RelatedServiceTypesString = value == null ? "" : string.Join(";", value.Select(v => ((int)v).ToString()));
        }
    }

    [Required]
    public string RelatedServiceTypesString { get; set; } = "";

    [Required]
    public ConversationType ConversationType { get; set; }

    public ICollection<ConversationMessageItem> Messages { get; set; } = new List<ConversationMessageItem>();

}
