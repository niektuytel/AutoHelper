using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Application.Conversations.Commands.SendMessage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.StartConversationItems;

public class StartConversationItemsValidator : AbstractValidator<StartConversationItemsCommand>
{
    private readonly IApplicationDbContext _context;

    public StartConversationItemsValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.ConversationIds)
            .NotEmpty().WithMessage("Should contain some Conversation ID.")
            .MustAsync(BeValidAndExistingConversation)
            .WithMessage("Invalid or non-existent conversation id(s).");
    }

    private async Task<bool> BeValidAndExistingConversation(StartConversationItemsCommand command, IEnumerable<Guid> conversations, CancellationToken cancellationToken)
    {
        var entities = _context.Conversations
            .AsNoTracking()
            .Include(x => x.Messages)
            .Where(x => conversations.Any(y => y == x.Id))
            .AsEnumerable();

        command.ConversationItems = entities;
        return entities?.Any() == true;
    }
}
