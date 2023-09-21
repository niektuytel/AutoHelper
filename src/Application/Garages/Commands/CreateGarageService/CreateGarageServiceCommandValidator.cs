using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
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
            .MustAsync(TitleNotAlreadyExist).WithMessage(c => $"A service with title {c.Type} already exists for this garage.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        RuleFor(v => v.DurationInMinutes)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(v => v.Price)
            .NotEmpty().WithMessage("Price is required.");

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty.")
            .CustomAsync(async (userId, context, cancellationToken) => {
                var garage = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
                if (garage == null)
                {
                    context.AddFailure("No garage found for this user.");
                }
                else
                {
                    // Attach garage to the command for further processing in the command handler
                    (context.InstanceToValidate as CreateGarageServiceCommand).UserGarage = garage;
                }
            });
    }

    private async Task<bool> TitleNotAlreadyExist(CreateGarageServiceCommand request, GarageServiceType type, CancellationToken cancellationToken)
    {
        var garage = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (garage == null) return true; // This check is redundant but keeping for clarity

        var entity = await _context.GarageServices.FirstOrDefaultAsync(x =>
            x.GarageId == garage.Id &&
            x.UserId == request.UserId &&
            x.Type == type
        );

        return entity == null;
    }
}
