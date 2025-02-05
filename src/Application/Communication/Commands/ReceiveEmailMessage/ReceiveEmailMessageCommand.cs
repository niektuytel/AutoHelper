﻿using System.Text.Json.Serialization;
using AutoHelper.Application.Messages.Commands.CreateConversationMessage;
using AutoHelper.Domain.Entities.Conversations;

using MediatR;

namespace AutoHelper.Application.Messages.Commands.ReceiveMessage;

public class ReceiveEmailMessageCommand : IRequest<ConversationMessageItem>
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

public class ReceiveMessageCommandHandler : IRequestHandler<ReceiveEmailMessageCommand, ConversationMessageItem>
{
    private readonly ISender _mediator;

    public ReceiveMessageCommandHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<ConversationMessageItem?> Handle(ReceiveEmailMessageCommand request, CancellationToken cancellationToken)
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
