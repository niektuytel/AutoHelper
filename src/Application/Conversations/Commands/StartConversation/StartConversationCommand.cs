using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Conversations.Commands.StartConversation;

public record StartConversationCommand : IRequest<ConversationItem>
{
    public Guid RelatedGarageLookupId { get; set; }

    public Guid RelatedVehicleLookupId { get; set; }

    public string? SenderWhatsAppNumber { get; set; }

    public string? SenderEmail { get; set; }

    public string? ReceiverWhatsAppNumber { get; set; }

    public string? ReceiverEmail { get; set; }

    public ConversationMessageType MessageType { get; set; }

    public string MessageContent { get; set; }
}

public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, ConversationItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StartConversationCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ConversationItem> Handle(StartConversationCommand request, CancellationToken cancellationToken)
    {
        //var entity = new GarageItem
        //{
        //    UserId = request.UserId,
        //    Name = request.Name,
        //    PhoneNumber = request.PhoneNumber,
        //    WhatsAppNumber = request.WhatsAppNumber,
        //    Email = request.Email,
        //    Location = new GarageLocationItem
        //    {
        //        Address = request.Location.Address,
        //        PostalCode = request.Location.PostalCode,
        //        City = request.Location.City,
        //        Country = request.Location.Country,
        //        Longitude = request.Location.Longitude,
        //        Latitude = request.Location.Latitude
        //    },
        //    BankingDetails = new GarageBankingDetailsItem
        //    {
        //        BankName = request.BankingDetails.BankName,
        //        KvKNumber = request.BankingDetails.KvKNumber,
        //        AccountHolderName = request.BankingDetails.AccountHolderName,
        //        IBAN = request.BankingDetails.IBAN
        //    },
        //    ServicesSettings = new GarageServicesSettingsItem()
        //};

        //// If you wish to use domain events, then you can add them here:
        //// entity.AddDomainEvent(new SomeDomainEvent(entity));

        //_context.Garages.Add(entity);
        //await _context.SaveChangesAsync(cancellationToken);
        //return entity;
        return null;
    }
}
