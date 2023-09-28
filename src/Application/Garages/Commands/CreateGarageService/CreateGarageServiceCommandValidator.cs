using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Garages;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;

public class CreateGarageServiceCommandValidator : AbstractValidator<CreateGarageServiceCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateGarageServiceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type is required.")
            .MustAsync(TitleNotAlreadyExist)
            .WithMessage(c => $"A service with title {c.Type} already exists for this garage.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(800).WithMessage("Description must not exceed 200 characters.");

        RuleFor(v => v.DurationInMinutes)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(v => v.Price)
            .NotEmpty().WithMessage("Price is required.");

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(async (userId, cancellationToken) =>
            {
                return await _context.Garages.AnyAsync(x => x.UserId == userId, cancellationToken);
            })
            .WithMessage("No garage found for this user.");
    }

    private async Task<bool> TitleNotAlreadyExist(CreateGarageServiceCommand request, GarageServiceType type, CancellationToken cancellationToken)
    {
        var foundSome = await _context.GarageServices.AnyAsync(x => x.UserId == request.UserId && x.Type == type, cancellationToken);
        return foundSome == false;
    }
}
