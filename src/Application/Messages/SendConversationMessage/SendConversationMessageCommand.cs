using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Messages.SendConfirmationMessage;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Messages.SendConversationMessage;

public record SendConversationMessageCommand : IRequest<SendConfirmationMessageCommand?>
{
    public SendConversationMessageCommand(
        Guid conversationId,
        ContactType senderContactType,
        string senderContactIdentifier,
        ContactType receiverContactType,
        string receiverContactIdentifier,
        string messageContent,
        int messageIndex = 1
    )
    {
        ConversationId = conversationId;
        SenderContactType = senderContactType;
        SenderContactIdentifier = senderContactIdentifier;
        ReceiverContactType = receiverContactType;
        ReceiverContactIdentifier = receiverContactIdentifier;
        MessageIndex = messageIndex;
        Timestamp = DateTime.UtcNow;
        Status = MessageStatus.Pending;
        MessageContent = messageContent;
    }

    public SendConversationMessageCommand(
        Guid conversationId,
        ContactType senderContactType,
        string senderContactIdentifier,
        ContactType receiverContactType,
        string receiverContactIdentifier,
        int messageIndex,
        DateTime dateTime,
        MessageStatus status,
        string messageContent,
        string? errorMessage
    )
    {
        ConversationId = conversationId;
        SenderContactType = senderContactType;
        SenderContactIdentifier = senderContactIdentifier;
        ReceiverContactType = receiverContactType;
        ReceiverContactIdentifier = receiverContactIdentifier;
        MessageIndex = messageIndex;
        Timestamp = dateTime;
        Status = status;
        MessageContent = messageContent;
        ErrorMessage = errorMessage;
    }

    public Guid ConversationId { get; private set; }

    public ContactType SenderContactType { get; private set; }
    public string SenderContactIdentifier { get; private set; }

    public ContactType ReceiverContactType { get; private set; }
    public string ReceiverContactIdentifier { get; private set; }

    public int MessageIndex { get; set; }
    public DateTime Timestamp { get; private set; }
    public MessageStatus Status { get; private set; }

    public string MessageContent { get; private set; }
    public string? ErrorMessage { get; private set; }

}

public class StartConversationCommandHandler : IRequestHandler<SendConversationMessageCommand, SendConfirmationMessageCommand?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StartConversationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SendConfirmationMessageCommand?> Handle(SendConversationMessageCommand request, CancellationToken cancellationToken)
    {


        // send confirmation message to the sender
        var confirmSendingCommand = new SendConfirmationMessageCommand(
            request.ConversationId,
            request.SenderContactType,
            request.SenderContactIdentifier,
            request.MessageContent
        );

        return confirmSendingCommand;
    }
}
