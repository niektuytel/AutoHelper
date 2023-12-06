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
            .MustAsync(ServiceShouldNotExist)
            .WithMessage(c => $"A service with Type: {c.Type}, VehicleType:{c.VehicleType} and Title:{c.Title} already exists for this garage.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(800).WithMessage("Description must not exceed 800 characters.");

        RuleFor(v => v.ExpectedNextDateIsRequired)
            .NotNull().WithMessage("ExpectedNextDateIsRequired is required.");

        RuleFor(v => v.ExpectedNextOdometerReadingIsRequired)
            .NotNull().WithMessage("ExpectedNextOdometerReadingIsRequired is required.");

    }

    private async Task<bool> ServiceShouldNotExist(CreateGarageServiceCommand request, GarageServiceType type, CancellationToken cancellationToken)
    {
        var foundSome = await _context.GarageServices.AnyAsync(x => 
            x.UserId == request.UserId && 
            x.Type == type && 
            x.VehicleType == request.VehicleType && 
            x.Title == request.Title
            , cancellationToken
        );
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
