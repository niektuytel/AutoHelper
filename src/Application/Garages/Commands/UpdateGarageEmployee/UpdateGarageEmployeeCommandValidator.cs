using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;

public class UpdateGarageEmployeeCommandValidator : AbstractValidator<UpdateGarageEmployeeCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGarageEmployeeCommandValidator(IApplicationDbContext context)
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
            .NotEmpty()
            .WithMessage("UserId cannot be empty.");

        RuleFor(v => v.Id)
            .NotEmpty()
            .WithMessage("Id cannot be empty.");

        // Custom rule that considers both UserId and EmployeeId
        RuleFor(v => v)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                var userId = request.UserId;
                var employeeId = request.Id;

                var garage = await _context.GarageEmployees.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == employeeId, cancellationToken);
                if (garage == null)
                {
                    context.AddFailure("No garage employee found for this user.");
                }
            });



    }
}
