using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Commands.DeleteGarageEmployee;


public record DeleteGarageEmployeeCommand : IRequest<GarageEmployeeItem>
{

    public DeleteGarageEmployeeCommand(Guid employeeId, string userId)
    {
        EmployeeId = employeeId;
        UserId = userId;
    }
    public Guid EmployeeId { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }
}

public class DeleteGarageEmployeeCommandHandler : IRequestHandler<DeleteGarageEmployeeCommand, GarageEmployeeItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteGarageEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageEmployeeItem> Handle(DeleteGarageEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageEmployees
            .Include(item => item.Contact)
            .Include(item => item.WorkSchema)
            .Include(item => item.WorkExperiences)
            .FirstAsync(item => item.Id == request.EmployeeId, cancellationToken: cancellationToken);

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.GarageEmployees.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }
}
