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

        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty.");

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
            .NotEmpty().WithMessage("Work Schema cannot be empty.")
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
            .NotEmpty().WithMessage("Work Experiences cannot be empty.")
            .ForEach(workExperienceItemRule =>
            {
                workExperienceItemRule.ChildRules(item =>
                {
                    item.RuleFor(we => we.ServiceId)
                        .NotNull()
                        .CustomAsync(async (serviceId, context, cancellationToken) => {
                            var service = await _context.GarageServices.FirstOrDefaultAsync(x => x.Id == serviceId, cancellationToken);
                            if (service == null)
                            {
                                context.AddFailure("No found defined service for this user.");
                            }
                        });

                    item.RuleFor(we => we.Description).NotEmpty().WithMessage("Description cannot be empty.");
                });
            });

        RuleFor(v => v.UserId)
            .CustomAsync(async (userId, context, cancellationToken) => {
                var garage = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
                if (garage == null)
                {
                    context.AddFailure("No garage found for this user.");
                }
                else
                {
                    // Attach garage to the command for further processing in the command handler
                    (context.InstanceToValidate as CreateGarageEmployeeCommand).UserGarage = garage;
                }
            });
    }
}
