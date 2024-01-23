﻿using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.SendConversationMessage;

public class SendConversationMessageValidator : AbstractValidator<SendConversationMessageCommand>
{
    private readonly IApplicationDbContext _context;

    public SendConversationMessageValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .MustAsync(BeValidAndExistingMessage)
            .WithMessage("Invalid or non-existent conversation message.");

    }

    private async Task<bool> BeValidAndExistingMessage(SendConversationMessageCommand command, CancellationToken cancellationToken)
    {
        if (command.Message == null && command.MessageId != default)
        {
            var entity = _context.ConversationMessages
                .AsNoTracking()
                .Include(x => x.Conversation)
                .ThenInclude(x => x.RelatedGarage)
                .Include(x => x.Conversation)
                .ThenInclude(x => x.RelatedVehicleLookup)
                .FirstOrDefault(x => x.Id == command.MessageId);

            command.Message = entity;
        }

        return command.Message != null;
    }
}
