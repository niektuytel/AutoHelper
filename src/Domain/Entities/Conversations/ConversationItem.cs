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
    [Required]
    public Guid RelatedVehicleLookupId { get; set; }

    [ForeignKey(nameof(RelatedVehicleLookupId))]
    public VehicleLookupItem RelatedVehicleLookup { get; set; } = null!;

    [Required]
    public Guid RelatedGarageLookupId { get; set; }
    
    [ForeignKey(nameof(RelatedGarageLookupId))]
    public GarageLookupItem RelatedGarageLookup { get; set; } = null!;

    public string? FromWhatsappNumber { get; set; }

    public string? FromEmailAddress { get; set; }

    public string? ToWhatsappNumber { get; set; }

    public string? ToEmailAddress { get; set; }

    [Required]
    public ConversationMessageType MessageType { get; set; }

    [Required]
    public string MessageContent { get; set; } = null!;

    public PriorityLevel Priority { get; set; } = PriorityLevel.Low;
}
