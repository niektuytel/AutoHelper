using System.Text.Json;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.ReviewVehicleServiceLog;

public class ReviewVehicleServiceLogCommandValidator : AbstractValidator<ReviewVehicleServiceLogCommand>
{
    private readonly IApplicationDbContext _context;

    public ReviewVehicleServiceLogCommandValidator(IApplicationDbContext applicationDbContext, IAesEncryptionService aesEncryptionService)
    {
        _context = applicationDbContext;


        RuleFor(x => x.ActionString)
            .NotEmpty()
            .MustAsync(async (command, actionString, cancellationToken) =>
            {
                var actionJson = aesEncryptionService.Decrypt(actionString);
                if (actionJson == null)
                {
                    return false;
                }

                var action = JsonSerializer.Deserialize<ServiceLogReviewAction>(actionJson!);
                if (action == null)
                {
                    return false;
                }

                var serviceLog = await _context.VehicleServiceLogs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == action.ServiceLogId);
                if (serviceLog == null)
                {
                    return false;
                }

                command.Approved = action.Approve;
                command.ServiceLog = serviceLog;
                return true;
            })
            .WithMessage("Service log not found");

    }

}
