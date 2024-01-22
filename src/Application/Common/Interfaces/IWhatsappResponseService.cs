using AutoHelper.Domain.Entities.Conversations;

namespace AutoHelper.Application.Common.Interfaces;

public interface IWhatsappResponseService
{
    Task UpdateMessageId(ConversationMessageItem message, string whatsappMessageId, CancellationToken cancellationToken, bool skipWhenExist = true);
    Task<Guid?> GetValidatedConversationId(string identifier, string messageId, string? contextMessageId = null);
    Task MarkMessageAsRead(string whatsappMessageId);
}