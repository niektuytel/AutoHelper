using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.DTOs;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
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
        var entity = await _context.GarageEmployees.FindAsync(request.Id);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageEmployeeItem), request.Id);
        }

        // Update fields
        entity.IsActive = request.IsActive;
        entity.Contact = request.Contact;

        // Update work schema
        var existingWorkSchemas = await _context.GarageEmployeeWorkSchemaItems
            .Where(w => w.EmployeeId == entity.Id)
            .ToListAsync(cancellationToken);

        _context.GarageEmployeeWorkSchemaItems.RemoveRange(existingWorkSchemas);
        entity.WorkSchema = request.WorkSchema.Select(item => new GarageEmployeeWorkSchemaItem
        {
            EmployeeId = entity.Id,
            WeekOfYear = item.WeekOfYear,
            DayOfWeek = item.DayOfWeek,
            StartTime = item.StartTime,
            EndTime = item.EndTime,
            Notes = item.Notes
        }).ToList();

        // Update work experiences
        var existingWorkExperiences = await _context.GarageEmployeeWorkExperienceItems
            .Where(w => w.EmployeeId == entity.Id)
            .ToListAsync(cancellationToken);

        _context.GarageEmployeeWorkExperienceItems.RemoveRange(existingWorkExperiences);
        entity.WorkExperiences = request.WorkExperiences.Select(item => new GarageEmployeeWorkExperienceItem
        {
            EmployeeId = entity.Id,
            ServiceId = item.ServiceId,
            Description = item.Description
        }).ToList();

        _context.GarageEmployees.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

}
