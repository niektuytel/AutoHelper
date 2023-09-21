using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.DeleteGarageEmployee;

public class DeleteGarageEmployeeCommandValidator : AbstractValidator<DeleteGarageEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteGarageEmployeeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");

        RuleFor(v => v.EmployeeId)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");
        
        // Custom rule that considers both UserId and EmployeeId
        RuleFor(v => v)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                var userId = request.UserId;
                var employeeId = request.EmployeeId;

                var garage = await _context.GarageEmployees.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == employeeId, cancellationToken);
                if (garage == null)
                {
                    context.AddFailure("No garage employee found for this user.");
                }
            });
    }
}
