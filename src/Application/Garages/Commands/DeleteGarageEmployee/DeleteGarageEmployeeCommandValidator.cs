using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using FluentValidation;
using MediatR;
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

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");

        RuleFor(v => v)
            .MustAsync(async (v, cancellationToken) =>
            {
                return await _context.GarageEmployees.AnyAsync(x => x.UserId == v.UserId && x.Id == v.EmployeeId, cancellationToken);
            })
            .WithMessage("No garage employee found for this user.");
    }
}
