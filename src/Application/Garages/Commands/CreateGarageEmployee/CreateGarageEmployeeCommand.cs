using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages.Commands.DTOs;
using AutoHelper.Application.Garages.Queries.GetGarageEmployees;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.CreateGarageEmployee;

public record CreateGarageEmployeeCommand : IRequest<GarageEmployeeItem>
{
    public bool IsActive { get; set; } = false;

    public ContactItem Contact { get; set; }

    public IEnumerable<GarageEmployeeWorkSchemaItemDto> WorkSchema { get; set; }

    public IEnumerable<GarageEmployeeWorkExperienceItemDto> WorkExperiences { get; set; }

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
            WorkSchema = new List<GarageEmployeeWorkSchemaItem>(),
            WorkExperiences = new List<GarageEmployeeWorkExperienceItem>()
        };

        // when has schema + experience, employee is active (in intial state)
        if (request.WorkSchema?.Any() == true && request.WorkExperiences?.Any() == true)
        {
            entity.IsActive = true;
        }

        // Guid will been used in work schema and work experience
        _context.GarageEmployees.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        entity.WorkSchema = request.WorkSchema.Select(item =>
        {
            return new GarageEmployeeWorkSchemaItem
            {
                EmployeeId = entity.Id,
                WeekOfYear = item.WeekOfYear,
                DayOfWeek = item.DayOfWeek,
                StartTime = item.StartTime,
                EndTime = item.EndTime,
                Notes = item.Notes
            };
        });
        entity.WorkExperiences = request.WorkExperiences.Select(item =>
        {
            return new GarageEmployeeWorkExperienceItem
            {
                EmployeeId = entity.Id,
                ServiceId = item.ServiceId,
                Description = item.Description
            };
        });


        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));
        
        // update employee
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

}
