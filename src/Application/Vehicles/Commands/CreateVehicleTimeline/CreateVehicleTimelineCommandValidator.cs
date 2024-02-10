using AutoHelper.Application.Common.Interfaces;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleTimeline;

public class CreateVehicleTimelineCommandValidator : AbstractValidator<CreateVehicleTimelineCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateVehicleTimelineCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

        RuleFor(x => x.ServiceLog.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.");
    }
}
