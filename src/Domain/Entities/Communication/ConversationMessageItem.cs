﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Domain.Common.Enums;
using AutoHelper.Domain.Entities.Communication;

namespace AutoHelper.Domain.Entities.Conversations;

public class ConversationMessageItem : BaseAuditableEntity
{
    [Required]
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Required on Whatsapp to know which message was sent
    /// </summary>
    public string? WhatsappMessageId { get; set; } = null!;

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
    public ConversationMessageStatus Status { get; set; }

    [Required]
    public string MessageContent { get; set; } = null!;

    public string? ErrorMessage { get; set; }
}