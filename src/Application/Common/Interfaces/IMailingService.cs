namespace AutoHelper.Application.Common.Interfaces;

public interface IMailingService : IMessagingService
{
    Task SendMessageRaw(string receiverIdentifier, Guid conversationId, string senderName, string message);
}
