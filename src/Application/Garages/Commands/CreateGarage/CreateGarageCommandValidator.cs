using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public class CreateGarageCommandValidator : AbstractValidator<CreateGarageCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateGarageCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.GarageLookupIdentifier)
            .NotEmpty().WithMessage("Garage identifier is required.")
            .MustAsync(BeValidAndExistingGarage)
            .WithMessage("Invalid or non-existent garage lookup."); ;

        RuleFor(v => v.PhoneNumber)
            .NotEmpty().WithMessage("PhoneNumber is required.");

        RuleFor(v => v.EmailAddress)
            .NotEmpty().WithMessage("Email is required.");

        RuleFor(v => v.Location)
            .SetValidator(new BriefLocationValidator());

    }

    private async Task<bool> BeValidAndExistingGarage(CreateGarageCommand command, string lookupIdentifier, CancellationToken cancellationToken)
    {
        var lookup = await _context.GarageLookups.FirstOrDefaultAsync(x => x.Identifier == lookupIdentifier, cancellationToken);
        command.GarageLookup = lookup;

        return lookup != null;
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


