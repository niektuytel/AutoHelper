using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages._DTOs;
using AutoHelper.Application.Messages.Commands.SendConversationMessage;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.CreateConversationMessage;

public class CreateConversationMessageValidator : AbstractValidator<CreateConversationMessageCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateConversationMessageValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x)
            .Must(BeValidAndExistingMessage)
            .WithMessage("Invalid or non-existent conversation message.");
    }

    private bool BeValidAndExistingMessage(CreateConversationMessageCommand command)
    {
        if(command.ConversationId == null)
        {
            return command.Conversation != null;
        }   

        var entity = _context.ConversationMessages
            .AsNoTracking()
            .Include(x => x.Conversation)
            .FirstOrDefault(x => x.Id == command.ConversationId);

        command.Conversation = entity?.Conversation;
        return entity != null;
    }

}
