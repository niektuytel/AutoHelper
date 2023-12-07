using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Vehicles.Queries.GetVehicleTimeline;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;

public class GetGarageServicesQueryValidator : AbstractValidator<GetGarageServicesQuery>
{
    private readonly IApplicationDbContext _context;

    public GetGarageServicesQueryValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(BeValidAndExistingGarageLookup)
            .WithMessage("No garage found for this user.");

        // Custom rule for processing and validating LicensePlate
        RuleFor(x => x.LicensePlate)
            .Custom((licensePlate, context) =>
            {
                // Ignore when no license plate is provided
                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    return;
                }

                // Replace spaces or hyphens with an empty string
                var processedLicensePlate = licensePlate.Replace(" ", "").Replace("-", "");
                context.InstanceToValidate.LicensePlate = processedLicensePlate;

            });


    }

    private async Task<bool> BeValidAndExistingGarageLookup(GetGarageServicesQuery command, string? userId, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(x => x.Services)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        command.Garage = entity;
        return command.Garage != null;
    }
}