using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;

namespace AutoHelper.Application.Messages.Commands.StartConversation;

public record StartConversationCommand : IQueueRequest<SendMessageCommand?>
{
    public StartConversationCommand(
        Guid relatedGarageLookupId,
        Guid relatedVehicleLookupId,
        GarageServiceType[] relatedServiceTypes,
        string? senderWhatsAppNumberOrEmail,
        string? receiverWhatsAppNumberOrEmail,
        ConversationType messageType,
        string messageContent
    )
    {
        RelatedGarageLookupId = relatedGarageLookupId;
        RelatedVehicleLookupId = relatedVehicleLookupId;
        RelatedServiceTypes = relatedServiceTypes;
        SenderWhatsAppNumberOrEmail = senderWhatsAppNumberOrEmail;
        ReceiverWhatsAppNumberOrEmail = receiverWhatsAppNumberOrEmail;
        ConversationType = messageType;
        MessageContent = messageContent;
    }

    public Guid RelatedGarageLookupId { get; set; }
    public GarageLookupItem RelatedGarage { get; internal set; }

    public Guid RelatedVehicleLookupId { get; set; }
    public VehicleLookupItem RelatedVehicle { get; internal set; }

    public GarageServiceType[] RelatedServiceTypes { get; set; }

    public string SenderWhatsAppNumberOrEmail { get; set; }
    public ContactType SenderContactType { get; internal set; }

    public string ReceiverWhatsAppNumberOrEmail { get; set; }
    public ContactType ReceiverContactType { get; internal set; }

    public ConversationType ConversationType { get; set; }

    public string MessageContent { get; set; }
    public IQueueService QueueingService { get; set; }
}

public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, SendMessageCommand?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ISender _mediator;

    public StartConversationCommandHandler(IApplicationDbContext context, IMapper mapper, ISender mediator)
    {
        _context = context;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<SendMessageCommand?> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = new ConversationItem
        {
            RelatedGarageLookupId = request.RelatedGarageLookupId,
            RelatedVehicleLookupId = request.RelatedVehicleLookupId,
            RelatedServiceTypes = request.RelatedServiceTypes,
            ConversationType = request.ConversationType,
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync(cancellationToken);

        // send message to the receiver
        var messageCommand = new SendMessageCommand(
            conversation.Id,
            request.SenderContactType,
            request.SenderWhatsAppNumberOrEmail,
            request.ReceiverContactType,
            request.ReceiverWhatsAppNumberOrEmail,
            request.MessageContent
        );

        return messageCommand;
    }

}
