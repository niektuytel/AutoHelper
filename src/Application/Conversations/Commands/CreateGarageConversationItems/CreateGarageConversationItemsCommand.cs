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
using AutoHelper.Application.Conversations._DTOs;
using Hangfire;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Conversations.Commands.SendMessage;
using System.Text.Json.Serialization;

namespace AutoHelper.Application.Conversations.Commands.CreateGarageConversationItems;

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

    public CreateGarageConversationBatchCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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

                CreateConversationMessage(conversation.Id, request, garage);
                await _context.SaveChangesAsync(cancellationToken);

                conversations.Add(conversation);
            }
        }

        return conversations.AsEnumerable();
    }

    private ConversationItem CreateConversation(
        ConversationType conversationType, 
        string licensePlate, 
        string garageIdentifier, 
        IEnumerable<Guid> serviceIds
    ) {
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

    private ConversationMessageItem CreateConversationMessage(Guid conversationId, CreateGarageConversationItemsCommand request, VehicleService garage)
    {
        var senderIdentifier = request.UserEmailAddress ?? "";
        if (string.IsNullOrWhiteSpace(senderIdentifier))
        {
            senderIdentifier = request.UserWhatsappNumber;
        };
        

        var receiverIdentifier = garage.ConversationEmailAddress;
        if (string.IsNullOrWhiteSpace(senderIdentifier))
        {
            receiverIdentifier = garage.ConversationWhatsappNumber;
        }

        var senderType = senderIdentifier.GetContactType();
        var receiverType = receiverIdentifier.GetContactType();
        var message = new ConversationMessageItem
        {
            ConversationId = conversationId,
            SenderContactType = senderType,
            SenderContactIdentifier = senderIdentifier,
            ReceiverContactType = receiverType,
            ReceiverContactIdentifier = receiverIdentifier,
            Status = MessageStatus.Pending,
            MessageContent = request.MessageContent
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.ConversationMessages.Add(message);
        return message;
    }

}
