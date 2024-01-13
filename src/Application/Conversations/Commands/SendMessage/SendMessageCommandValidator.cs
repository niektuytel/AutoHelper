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

        RuleFor(x => x.ConversationMessageId)
            .NotEmpty().WithMessage("Conversation message ID is required.")
            .MustAsync(BeValidAndExistingMessage)
            .WithMessage("Invalid or non-existent conversation message.");

    }

    private async Task<bool> BeValidAndExistingMessage(SendMessageCommand command, Guid messageId, CancellationToken cancellationToken)
    {
        var entity = _context.ConversationMessages
            .AsNoTracking()
            .Include(x => x.Conversation)
            .ThenInclude(x => x.RelatedGarage)
            .Include(x => x.Conversation)
            .ThenInclude(x => x.RelatedVehicleLookup)
            .FirstOrDefault(x => x.Id == messageId);

        command.ConversationMessage = entity;
        return entity != null;
    }
}
