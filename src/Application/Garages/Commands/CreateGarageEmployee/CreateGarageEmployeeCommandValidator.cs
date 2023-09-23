using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageEmployee;

public class CreateGarageEmployeeCommandValidator : AbstractValidator<CreateGarageEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateGarageEmployeeCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.Contact)
            .NotNull().WithMessage("Contact information cannot be null.")
            .ChildRules(contactRules =>
            {
                contactRules.RuleFor(c => c.FullName).NotEmpty().WithMessage("Full Name cannot be empty.");
                contactRules.RuleFor(c => c.PhoneNumber).NotEmpty().WithMessage("Phone Number cannot be empty.");
                contactRules.RuleFor(c => c.Email).NotEmpty().WithMessage("Email cannot be empty.");
            });

        RuleFor(v => v.WorkSchema)
            .NotNull().WithMessage("Work Schema cannot be null.")
            .ForEach(workSchemaItemRule =>
            {
                workSchemaItemRule.ChildRules(item =>
                {
                    item.RuleFor(ws => ws.DayOfWeek).NotNull();
                    item.RuleFor(ws => ws.StartTime).NotNull();
                    item.RuleFor(ws => ws.EndTime).NotNull();
                }); 
            });

        RuleFor(v => v.WorkExperiences)
            .NotNull().WithMessage("Work Experiences cannot be null.")
            .ForEach(workExperienceItemRule =>
            {
                workExperienceItemRule.ChildRules(item =>
                {
                    item.RuleFor(we => we.ServiceId)
                        .NotNull()
                        .MustAsync(async (serviceId, cancellationToken) =>
                        {
                            return await _context.GarageServices.AnyAsync(x => x.Id == serviceId, cancellationToken);
                        })
                        .WithMessage("No found defined service for this user.");

                });
            });

        RuleFor(v => v.UserId)
            .NotEmpty()
            .WithMessage("UserId cannot be empty.")
            .MustAsync(async (userId, cancellationToken) =>
            {
                return await _context.Garages.AnyAsync(x => x.UserId == userId, cancellationToken);
            })
            .WithMessage("No garage found for this user.");
    }
}
