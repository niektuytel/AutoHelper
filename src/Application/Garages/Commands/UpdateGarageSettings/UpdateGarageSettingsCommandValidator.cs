using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;

public class UpdateGarageSettingsCommandValidator : AbstractValidator<UpdateGarageSettingsCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGarageSettingsCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.");

        RuleFor(v => v.EmailAddress)
            .NotEmpty().WithMessage("EmailAddress is required.");

        RuleFor(v => v.ConversationEmail)
            .NotEmpty().WithMessage("ConversationEmail is required.");

        RuleFor(v => v.ConversationWhatsappNumber)
            .NotEmpty().WithMessage("ConversationWhatsappNumber is required.");

        // Conditional validation for Location
        When(v => v.Location != null, () =>
        {
            RuleFor(v => v.Location).SetValidator(new BriefLocationValidator());
        });
    }

    private async Task<bool> BeValidAndExistingGarageLookup(UpdateGarageSettingsCommand command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }

    public class BriefLocationValidator : AbstractValidator<GarageLocationDtoItem>
    {
        public BriefLocationValidator()
        {
            RuleFor(v => v.Address)
                .NotEmpty().WithMessage("Address is required.");

            RuleFor(v => v.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(v => v.Longitude)
                .NotEmpty().WithMessage("Longitude is required.");

            RuleFor(v => v.Latitude)
                .NotEmpty().WithMessage("Latitude is required.");

        }
    }


}
