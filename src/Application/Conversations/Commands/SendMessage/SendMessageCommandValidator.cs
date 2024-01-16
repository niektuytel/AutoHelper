using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    private readonly IApplicationDbContext _context;

    public SendMessageCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(BeValidAndExistingMessage)
            .WithMessage("Invalid or non-existent conversation message.");

    }

    private async Task<bool> BeValidAndExistingMessage(SendMessageCommand command, CancellationToken cancellationToken)
    {
        if (command.ConversationMessageId == null)
        {
            return command.ConversationMessage != null;
        }

        var entity = _context.ConversationMessages
            .AsNoTracking()
            .Include(x => x.Conversation)
            .ThenInclude(x => x.RelatedGarage)
            .Include(x => x.Conversation)
            .ThenInclude(x => x.RelatedVehicleLookup)
            .FirstOrDefault(x => x.Id == command.ConversationMessageId);

        command.ConversationMessage = entity;
        return entity != null;
    }
}
