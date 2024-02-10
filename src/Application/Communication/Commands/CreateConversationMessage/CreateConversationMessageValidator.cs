using AutoHelper.Application.Common.Interfaces;

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
        if (command.ConversationId == null)
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
