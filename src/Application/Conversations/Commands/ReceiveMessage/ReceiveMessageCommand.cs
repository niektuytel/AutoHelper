using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Conversations.Commands.CreateConversationMessage;
using AutoHelper.Application.Conversations.Commands.SendMessage;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Messaging.Interfaces;
using MediatR;

namespace AutoHelper.Application.Conversations.Commands.ReceiveMessage;

public class ReceiveMessageCommand : IRequest<ConversationMessageItem>
{
    public string From { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    [JsonIgnore]
    internal string ReceiverIdentifier { get; set; } = null!;

    [JsonIgnore]
    internal string SenderContactIdentifier { get; set; } = null!;

    [JsonIgnore]
    internal ConversationItem? Conversation { get; set; } = null!;


}

public class ReceiveMessageCommandHandler : IRequestHandler<ReceiveMessageCommand, ConversationMessageItem>
{
    private readonly ISender _mediator;
    private readonly IEmailHelper _emailHelper;

    public ReceiveMessageCommandHandler(ISender mediator, IEmailHelper emailHelper)
    {
        _mediator = mediator;
        _emailHelper = emailHelper;
    }

    public async Task<ConversationMessageItem?> Handle(ReceiveMessageCommand request, CancellationToken cancellationToken)
    {
        var createMessage = new CreateConversationMessageCommand(request.Conversation!)
        {
            SenderIdentifier = request.SenderContactIdentifier,
            ReceiverIdentifier = request.ReceiverIdentifier,
            Message = request.Body
        };

        var message = await _mediator.Send(createMessage, cancellationToken);
        return message;
    }
}
