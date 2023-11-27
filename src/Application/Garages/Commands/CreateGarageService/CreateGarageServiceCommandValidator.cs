using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
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

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(v => v.Type)
            .NotEmpty().WithMessage("Type is required.")
            .MustAsync(TitleNotAlreadyExist)
            .WithMessage(c => $"A service with title {c.Type} already exists for this garage.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(800).WithMessage("Description must not exceed 200 characters.");

    }

    private async Task<bool> TitleNotAlreadyExist(CreateGarageServiceCommand request, GarageServiceType type, CancellationToken cancellationToken)
    {
        var foundSome = await _context.GarageServices.AnyAsync(x => x.UserId == request.UserId && x.Type == type, cancellationToken);
        return foundSome == false;
    }

    private async Task<bool> BeValidAndExistingGarageLookup(CreateGarageServiceCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

}
