

using AutoHelper.Application.Common.Enums;

namespace AutoHelper.Application.Common.Interfaces;

public interface IWhatsappResponseService
{
    Task<Guid?> GetConversationId(string messageId);
    Task SetMessageIdWhenEmpty(Guid conversationMessageId, string messageId, CancellationToken cancellationToken);
    Task MarkMessageAsRead(string messageId);
    Task SendErrorMessage(WhatsappMessageErrorType type);
}