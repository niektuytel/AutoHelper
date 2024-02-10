using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.ReceiveMessage;

using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.ReceiveWhatsappMessage;


public class ReceiveWhatsappMessageValidator : AbstractValidator<ReceiveWhatsappMessageCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentificationHelper _phoneNumberHelper;

    public ReceiveWhatsappMessageValidator(IApplicationDbContext context, IIdentificationHelper phoneNumberHelper)
    {
        _context = context;
        _phoneNumberHelper = phoneNumberHelper;

        RuleFor(command => command)
            .Must(ContainsValidId)
            .WithMessage("Invalid or non-existent ID in the message body, shoul have some 'Referentie-ID: ...'")
            .Must(IsValidFromIdentifier)
            .WithMessage("Invalid 'From' identifier.");
    }

    private bool ContainsValidId(ReceiveWhatsappMessageCommand command)
    {
        if (command.ConversationId == default && command?.Conversation?.Id == null)
        {
            return false;
        }

        var entity = _context.Conversations
            .AsNoTracking()
            .Include(x => x.RelatedGarage)
            .Include(x => x.RelatedVehicleLookup)
            .Include(x => x.Messages)
            .FirstOrDefault(cm => cm.Id == command.ConversationId);

        command.Conversation = entity;
        return entity != null;
    }

    private bool IsValidFromIdentifier(ReceiveWhatsappMessageCommand command)
    {
        if (command.Conversation == null)
        {
            return false;
        }

        var lastMessage = command.Conversation!.Messages.LastOrDefault();
        if (lastMessage == null)
        {
            return false;
        }

        if (lastMessage.SenderContactIdentifier.Equals(command.From))
        {
            command.SenderContactIdentifier = lastMessage.SenderContactIdentifier;
            command.ReceiverIdentifier = lastMessage.ReceiverContactIdentifier;
        }
        else if (lastMessage.ReceiverContactIdentifier.Equals(command.From))
        {
            command.SenderContactIdentifier = lastMessage.ReceiverContactIdentifier;
            command.ReceiverIdentifier = lastMessage.SenderContactIdentifier;
        }
        else
        {
            return false;
        }

        return true;
    }

}
