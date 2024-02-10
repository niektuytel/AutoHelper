using AutoHelper.Application.Common.Interfaces;
using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleTimeline;

public class DeleteVehicleTimelineCommandValidator : AbstractValidator<DeleteVehicleTimelineCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteVehicleTimelineCommandValidator(IApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;

    }
}
