using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.DTOs;
using AutoHelper.Application.Garages.Queries.GetGarageEmployees;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageEmployee;

public record CreateGarageEmployeeCommand : IRequest<GarageEmployeeItem>
{
    public bool IsActive { get; set; } = false;

    public ContactItem Contact { get; set; }

    public IEnumerable<GarageEmployeeWorkSchemaItemDto> WorkSchema { get; set; } = new List<GarageEmployeeWorkSchemaItemDto>();

    public IEnumerable<GarageEmployeeWorkExperienceItemDto> WorkExperiences { get; set; } = new List<GarageEmployeeWorkExperienceItemDto>();

    [JsonIgnore]
    public string UserId { get; set; }

}

public class CreateGarageEmployeeItemCommandHandler : IRequestHandler<CreateGarageEmployeeCommand, GarageEmployeeItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGarageEmployeeItemCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageEmployeeItem> Handle(CreateGarageEmployeeCommand request, CancellationToken cancellationToken)
    {
        var garageEntity = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);
        if (garageEntity == null)
        {
            throw new NotFoundException($"{nameof(GarageItem)} on UserId:", request.UserId);
        }

        var entity = new GarageEmployeeItem
        {
            UserId = request.UserId,
            GarageId = garageEntity.Id,
            Contact = request.Contact,
            IsActive = true,
            WorkSchema = new List<GarageEmployeeWorkSchemaItem>(),
            WorkExperiences = new List<GarageEmployeeWorkExperienceItem>()
        };

        // Guid will been used in work schema and work experience
        _context.GarageEmployees.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

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

        if (request.WorkExperiences?.Any() == true)
        {
            entity.WorkExperiences = request.WorkExperiences.Select(item => new GarageEmployeeWorkExperienceItem
            {
                GarageId = garageEntity.Id,
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
        
        // update employee
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

}
