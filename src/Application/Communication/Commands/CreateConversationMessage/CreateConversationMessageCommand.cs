using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Common.Enums;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using MediatR;

namespace AutoHelper.Application.Messages.Commands.CreateConversationMessage;

public record CreateConversationMessageCommand : IRequest<ConversationMessageItem>
{
    public CreateConversationMessageCommand(Guid conversationId)
    {
        ConversationId = conversationId;
    }

    public CreateConversationMessageCommand(ConversationItem conversation)
    {
        Conversation = conversation;
    }

    public Guid? ConversationId { get; set; } = null;

    public string? WhatsappMessageId { get; set; } = null;

    /// <summary>
    /// Should always been set, even when the  ConversationId is set.
    /// </summary>
    public ConversationItem? Conversation { get; set; } = null!;

    public string? SenderIdentifier { get; set; }

    public string? ReceiverIdentifier { get; set; }

    public string Message { get; set; } = string.Empty;
}

public class CreateGarageConversationBatchCommandHandler : IRequestHandler<CreateConversationMessageCommand, ConversationMessageItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _phoneNumberHelper;

    public CreateGarageConversationBatchCommandHandler(IApplicationDbContext context, IIdentificationHelper phoneNumberHelper)
    {
        _context = context;
        _phoneNumberHelper = phoneNumberHelper;
    }

    public async Task<ConversationMessageItem> Handle(CreateConversationMessageCommand request, CancellationToken cancellationToken)
    {
        var senderType = request.SenderIdentifier!.GetContactType();
        if (senderType == ContactType.WhatsApp)
        {
            request.SenderIdentifier = _phoneNumberHelper.GetPhoneNumberId(request.SenderIdentifier!);
        }

        var receiverType = request.ReceiverIdentifier!.GetContactType();
        if (receiverType == ContactType.WhatsApp)
        {
            request.ReceiverIdentifier = _phoneNumberHelper.GetPhoneNumberId(request.ReceiverIdentifier!);
        }

        var message = new ConversationMessageItem
        {
            ConversationId = request.Conversation!.Id,
            WhatsappMessageId = request.WhatsappMessageId,
            SenderContactType = senderType,
            SenderContactIdentifier = request.SenderIdentifier!,
            ReceiverContactType = receiverType,
            ReceiverContactIdentifier = request.ReceiverIdentifier!,
            Status = ConversationMessageStatus.Pending,
            MessageContent = request.Message
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.ConversationMessages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return message;
    }

}
