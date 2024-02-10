using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.DeleteNotification;

public class DeleteNotificationValidator : AbstractValidator<DeleteNotificationCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteNotificationValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(x => x)
            .MustAsync(BeValidAndExistingIdentifier)
            .WithMessage("Invalid or non-existent Notification on identifier");

    }

    private async Task<bool> BeValidAndExistingIdentifier(DeleteNotificationCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.Notifications.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        command.Notification = entity;
        return entity != null;
    }
}
