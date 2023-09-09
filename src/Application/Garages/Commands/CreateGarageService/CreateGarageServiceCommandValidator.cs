using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;

public class CreateGarageServiceCommandValidator : AbstractValidator<CreateGarageServiceCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateGarageServiceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
            .MustAsync(TitleNotAlreadyExist).WithMessage(c => $"A service with title {c.Title} already exists for this user.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(200).WithMessage("Description must not exceed 200 characters.");

        RuleFor(v => v.Duration)
            .NotEmpty().WithMessage("Duration is required.");

        RuleFor(v => v.Price)
            .NotEmpty().WithMessage("Price is required.");

        RuleFor(v => v.UserId)
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

    private async Task<bool> TitleNotAlreadyExist(CreateGarageServiceCommand request, string title, CancellationToken cancellationToken)
    {
        var garage = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (garage == null) return true; // This check is redundant but keeping for clarity

        var entity = await _context.GarageServices.FirstOrDefaultAsync(x =>
            x.GarageId == garage.Id &&
            x.UserId == request.UserId &&
            x.Title == title
        );

        return entity == null;
    }
}
