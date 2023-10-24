using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Messages.SendConfirmationMessage;

public class SendConfirmationMessageCommand : IRequest<bool?>
{
    public SendConfirmationMessageCommand(
        Guid conversationId,
        string sendToName,
        ContactType contactType, 
        string contactIdentifier, 
        string messageContent
    ) {
        ConversationId = conversationId;
        SendToName = sendToName;
        ContactType = contactType;
        ContactIdentifier = contactIdentifier;
        MessageContent = messageContent;
    }

    public Guid ConversationId { get; set; }

    public string SendToName { get; set; }

    public ContactType ContactType { get; private set; }

    public string ContactIdentifier { get; private set; }

    public string MessageContent { get; set; }
}

public class SendConfirmationMessageCommandHandler : IRequestHandler<SendConfirmationMessageCommand, bool?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IWhatsappService _whatsappService;
    private readonly IMailingService _mailingService;

    public SendConfirmationMessageCommandHandler(
        IApplicationDbContext context, 
        IMapper mapper, 
        IWhatsappService whatsappService, 
        IMailingService mailingService
    ){
        _context = context;
        _mapper = mapper;
        _whatsappService = whatsappService;
        _mailingService = mailingService;
    }

    public async Task<bool?> Handle(SendConfirmationMessageCommand request, CancellationToken cancellationToken)
    {
        var subject = "AutoHelper - Je bericht is successvol gestuurd naar de garage.";
        var message = $"Hallo, We hebben je bericht verstuurd en hopen op zo snel mogelijk een antwoord te hebben.\n\n Het gaat om het bericht: \n'{request.MessageContent}'";

        if (request.ContactType == ContactType.Email)
        {
            await _mailingService.SendEmailAsync(request.ContactIdentifier, subject, message);
        }
        else if(request.ContactType == ContactType.WhatsApp)
        {
            await _whatsappService.SendConfirmationMessageAsync(request.ContactIdentifier, request.ConversationId, request.SendToName);
        }
        else
        {
            throw new Exception($"Invalid contact type: {request.ContactType}");
        }

        return null;
    }
}

