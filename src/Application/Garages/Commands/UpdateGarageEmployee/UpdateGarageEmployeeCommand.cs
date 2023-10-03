using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.DTOs;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;


public record UpdateGarageEmployeeCommand : IRequest<GarageEmployeeItem>
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; } = false;

    public ContactItem Contact { get; set; }

    public IEnumerable<GarageEmployeeWorkSchemaItemDto> WorkSchema { get; set; }

    public IEnumerable<GarageEmployeeWorkExperienceItemDto> WorkExperiences { get; set; }

    [JsonIgnore]
    public string? UserId { get; set; }

}

public class UpdateGarageEmployeeCommandHandler : IRequestHandler<UpdateGarageEmployeeCommand, GarageEmployeeItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateGarageEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<GarageEmployeeItem> Handle(UpdateGarageEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageEmployees
            .Include(item => item.WorkSchema)
            .Include(item => item.WorkExperiences)
            .FirstOrDefaultAsync(item => item.Id == request.Id && item.UserId == request.UserId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageEmployeeItem), request.Id);
        }

        // Update fields
        entity.Contact = request.Contact;
        entity.IsActive = request.IsActive;
        
        _context.GarageEmployeeWorkSchemaItems.RemoveRange(entity.WorkSchema);
        if (request.WorkSchema?.Any() == true)
        {
            entity.WorkSchema = request.WorkSchema.Select(item => new GarageEmployeeWorkSchemaItem
            {
                EmployeeId = entity.Id,
                WeekOfYear = item.WeekOfYear,
                DayOfWeek = item.DayOfWeek,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Notes = item.Notes
            }).ToList();
        }
        else
        {
            entity.WorkSchema = new List<GarageEmployeeWorkSchemaItem>();
            entity.IsActive = false;
        }

        _context.GarageEmployeeWorkExperienceItems.RemoveRange(entity.WorkExperiences);
        if(request.WorkExperiences?.Any() == true)
        {
            entity.WorkExperiences = request.WorkExperiences.Select(item => new GarageEmployeeWorkExperienceItem
            {
                GarageId = entity.GarageId,
                EmployeeId = entity.Id,
                ServiceId = item.ServiceId,
                Description = item.Description
            }).ToList();
        }
        else
        {
            entity.WorkExperiences = new List<GarageEmployeeWorkExperienceItem>();
            entity.IsActive = false;
        }

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        // Since we fetched the entity directly from the DbContext, it's already tracked. 
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

}
