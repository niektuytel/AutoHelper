using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationItem : BaseAuditableEntity
{
    public Guid RelatedGarageLookupId { get; set; }

    public Guid RelatedVehicleLookupId { get; set; }

    public string? SenderWhatsAppNumber { get; set; }

    public string? SenderEmail { get; set; }

    public string? ReceiverWhatsAppNumber { get; set; }

    public string? ReceiverEmail { get; set; }

    public ConversationMessageType MessageType { get; set; }

    public string MessageContent { get; set; }
}
