using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationItem : BaseAuditableEntity
{
    public PriorityLevel Priority { get; set; } = PriorityLevel.Low;

    [Required]
    public Guid RelatedVehicleLookupId { get; set; }

    [ForeignKey(nameof(RelatedVehicleLookupId))]
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
    public ConversationMessageType MessageType { get; set; }

    [Required]
    public string MessageContent { get; set; } = null!;

}
