using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Conversations.Commands.StartConversation;

public record StartConversationCommand : IRequest
{
    public StartConversationCommand(
        Guid relatedGarageLookupId, 
        Guid relatedVehicleLookupId, 
        GarageServiceType[] relatedServiceTypes,
        string? senderWhatsAppNumberOrEmail, 
        string? receiverWhatsAppNumberOrEmail, 
        ConversationMessageType messageType, 
        string messageContent
    ) {
        RelatedGarageLookupId = relatedGarageLookupId;
        RelatedVehicleLookupId = relatedVehicleLookupId;
        RelatedServiceTypes = relatedServiceTypes;
        SenderWhatsAppNumberOrEmail = senderWhatsAppNumberOrEmail;
        ReceiverWhatsAppNumberOrEmail = receiverWhatsAppNumberOrEmail;
        MessageType = messageType;
        MessageContent = messageContent;
    }

    public Guid RelatedGarageLookupId { get; set; }
    public GarageLookupItem RelatedGarage { get; internal set; }

    public Guid RelatedVehicleLookupId { get; set; }
    public VehicleLookupItem RelatedVehicle { get; internal set; }

    public GarageServiceType[] RelatedServiceTypes { get; set; }

    public string SenderWhatsAppNumberOrEmail { get; set; }

    public string ReceiverWhatsAppNumberOrEmail { get; set; }

    public ConversationMessageType MessageType { get; set; }

    public string MessageContent { get; set; }
}

public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StartConversationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        var entity = new ConversationItem
        {
            RelatedGarageLookupId = request.RelatedGarageLookupId,
            RelatedVehicleLookupId = request.RelatedVehicleLookupId,
            RelatedServiceTypes = request.RelatedServiceTypes,
            MessageType = request.MessageType,
            MessageContent = request.MessageContent
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Conversations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        // Detect using whatsapp or email and set on enum, crerate the enum
        var senderUseWhatsApp = true;
        if (Regex.IsMatch(request.SenderWhatsAppNumberOrEmail, StartConversationCommandValidator.EmailPattern))
        {
            senderUseWhatsApp = false;
        }


        // Send message to receiver
        // Send confirmation message to sender

        return Unit.Value;
    }
}
