using AutoHelper.Application.Common.Interfaces;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageReviewNotifier;

public class SendGarageServiceReviewValidator : AbstractValidator<SendGarageServiceReviewCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SendGarageServiceReviewValidator(IApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _context = applicationDbContext;
        _mapper = mapper;

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.GarageIdentifier)
            .NotEmpty().WithMessage("Garage identifier is required.")
            .MustAsync(BeValidAndExistingGarage)
            .WithMessage("Invalid or non-existent garage.");

        RuleFor(x => x.ServiceLogId)
            .NotEmpty().WithMessage("Garage service ID is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }

    private async Task<bool> BeValidAndExistingVehicle(SendGarageServiceReviewCommand command, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        command.LicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
        return vehicle != null;
    }

    private async Task<bool> BeValidAndExistingGarage(SendGarageServiceReviewCommand command, string lookupIdentifier, CancellationToken cancellationToken)
    {
        var garage = await _context.GarageLookups.FirstOrDefaultAsync(x => x.Identifier == lookupIdentifier, cancellationToken);
        command.Garage = garage;

        return command.Garage != null;
    }

}
