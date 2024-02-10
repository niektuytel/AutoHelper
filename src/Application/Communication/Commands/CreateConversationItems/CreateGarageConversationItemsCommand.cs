using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Messages.Commands.CreateConversationMessage;
using AutoHelper.Domain.Entities.Communication;
using AutoHelper.Domain.Entities.Conversations;
using MediatR;

namespace AutoHelper.Application.Messages.Commands.CreateGarageConversationItems;

public record CreateGarageConversationItemsCommand : IRequest<IEnumerable<ConversationItem>>
{
    public string? UserWhatsappNumber { get; set; }

    public string? UserEmailAddress { get; set; }

    public ConversationType MessageType { get; set; }

    public string MessageContent { get; set; } = string.Empty;

    public IEnumerable<VehicleService> Services { get; set; } = new List<VehicleService>();
}

public class CreateGarageConversationBatchCommandHandler : IRequestHandler<CreateGarageConversationItemsCommand, IEnumerable<ConversationItem>>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _identificationHelper;
    private readonly ISender _mediator;

    public CreateGarageConversationBatchCommandHandler(IApplicationDbContext context, IIdentificationHelper identificationHelper, ISender mediator)
    {
        _context = context;
        _identificationHelper = identificationHelper;
        _mediator = mediator;
    }

    public async Task<IEnumerable<ConversationItem>> Handle(CreateGarageConversationItemsCommand request, CancellationToken cancellationToken)
    {
        var conversations = new List<ConversationItem>();
        var vehicles = request.Services
            .DistinctBy(item => item.VehicleLicensePlate);

        foreach (var vehicle in vehicles)
        {
            var vehicleServices = request.Services
                .Where(item => item.VehicleLicensePlate == vehicle.VehicleLicensePlate);

            var garages = vehicleServices
                .DistinctBy(item => item.RelatedGarageLookupIdentifier);

            foreach (var garage in garages)
            {
                var serviceIds = vehicleServices
                    .Where(item => item.RelatedGarageLookupIdentifier == garage.RelatedGarageLookupIdentifier)
                    .Select(item => item.GarageServiceId);

                var conversation = CreateConversation(
                    request.MessageType,
                    vehicle.VehicleLicensePlate!,
                    garage.RelatedGarageLookupIdentifier!,
                    serviceIds
                );

                await CreateConversationMessage(conversation, request, garage, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                conversations.Add(conversation);
            }
        }

        return conversations.AsEnumerable();
    }

    private ConversationItem CreateConversation(ConversationType conversationType, string licensePlate, string garageIdentifier, IEnumerable<Guid> serviceIds)
    {
        var conversation = new ConversationItem
        {
            GarageLookupIdentifier = garageIdentifier!,
            VehicleLicensePlate = licensePlate,
            RelatedServiceIds = serviceIds.ToArray(),
            ConversationType = conversationType,
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Conversations.Add(conversation);
        return conversation;
    }

    private async Task<ConversationMessageItem> CreateConversationMessage(ConversationItem conversation, CreateGarageConversationItemsCommand request, VehicleService garage, CancellationToken token)
    {
        var senderIdentifier = _identificationHelper.GetValidIdentifier(request.UserEmailAddress, request.UserWhatsappNumber);
        var receiverIdentifier = _identificationHelper.GetValidIdentifier(garage.ConversationEmailAddress, garage.ConversationWhatsappNumber);

        var command = new CreateConversationMessageCommand(conversation)
        {
            SenderIdentifier = senderIdentifier!,
            ReceiverIdentifier = receiverIdentifier!,
            Message = request.MessageContent
        };

        var message = await _mediator.Send(command, token);
        return message;
    }

}
