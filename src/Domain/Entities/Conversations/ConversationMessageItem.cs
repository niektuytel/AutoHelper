using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Entities.Conversations.Enums;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationMessageItem : BaseAuditableEntity
{
    [Required]
    public Guid ConversationId { get; set; }

    [ForeignKey(nameof(ConversationId))]
    public ConversationItem Conversation { get; set; } = null!;

    [Required]
    public ContactType SenderContactType { get; set; }

    [Required]
    public string SenderContactIdentifier { get; set; } = null!;

    [Required]
    public ContactType ReceiverContactType { get; set; }

    [Required]
    public string ReceiverContactIdentifier { get; set; } = null!;

    [Required]
    public MessageStatus Status { get; set; }

    [Required]
    public string MessageContent { get; set; } = null!;

    public string? ErrorMessage { get; set; }
}