using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Conversations.Commands.CreateConversationMessage;
using AutoHelper.Application.Conversations.Commands.SendConversationMessage;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using MediatR;

namespace AutoHelper.Application.Conversations.Commands.ReceiveMessage;

public class ReceiveWhatsappMessageCommand : IRequest<ConversationMessageItem>
{

    public ReceiveWhatsappMessageCommand()
    {

    }

    public Guid ConversationId { get; set; }
    public string WhatsappMessageId { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    [JsonIgnore]
    internal string ReceiverIdentifier { get; set; } = null!;

    [JsonIgnore]
    internal string SenderContactIdentifier { get; set; } = null!;

    [JsonIgnore]
    internal ConversationItem? Conversation { get; set; } = null!;

}

public class ReceiveWhatsappMessageCommandHandler : IRequestHandler<ReceiveWhatsappMessageCommand, ConversationMessageItem>
{
    private readonly ISender _mediator;

    public ReceiveWhatsappMessageCommandHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<ConversationMessageItem?> Handle(ReceiveWhatsappMessageCommand request, CancellationToken cancellationToken)
    {
        var createMessage = new CreateConversationMessageCommand(request.Conversation!)
        {
            WhatsappMessageId = request.WhatsappMessageId,
            SenderIdentifier = request.SenderContactIdentifier,
            ReceiverIdentifier = request.ReceiverIdentifier,
            Message = request.Body
        };

        var message = await _mediator.Send(createMessage, cancellationToken);
        return message;
    }
}
