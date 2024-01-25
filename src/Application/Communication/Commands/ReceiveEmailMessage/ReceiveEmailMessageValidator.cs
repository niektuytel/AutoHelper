using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.ReceiveMessage;


public class ReceiveEmailMessageValidator : AbstractValidator<ReceiveEmailMessageCommand>
{
    private readonly IApplicationDbContext _context;

    public ReceiveEmailMessageValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(command => command)
            .Must(ContainsValidId)
            .WithMessage("Invalid or non-existent ID in the message body, shoul have some 'Referentie-ID: ...'")
            .Must(IsValidFromIdentifier)
            .WithMessage("Invalid 'From' identifier.");
    }

    private bool ContainsValidId(ReceiveEmailMessageCommand command)
    {
        var match = Regex.Match(command.Body.ToLower(), @"id:\s*([a-f\d]{8})");
        if (!match.Success)
        {
            return false;
        }

        var idValue = match.Groups[1].Value;
        if (!string.IsNullOrEmpty(idValue))
        {
            var entity = _context.Conversations
                .AsNoTracking()
                .Include(x => x.RelatedGarage)
                .Include(x => x.RelatedVehicleLookup)
                .Include(x => x.Messages)
                .FirstOrDefault(cm => cm.Id.ToString().StartsWith(idValue));

            command.Conversation = entity;
            return entity != null;
        }

        return false;
    }

    private bool IsValidFromIdentifier(ReceiveEmailMessageCommand command)
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
