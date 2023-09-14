using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;


public record UpdateGarageEmployeeCommand : IRequest<GarageEmployeeItem>
{
    public Guid Id { get; set; }

    [JsonIgnore]
    public string UserId { get; set; }


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
        var garageEntity = await _context.Garages.FindAsync(request.UserId);
        if (garageEntity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        throw new NotImplementedException();
        //garageEntity.Type = request.Type;
        //garageEntity.Description = request.Description;
        //garageEntity.DurationInMinutes = request.DurationInMinutes;
        //garageEntity.Price = request.Price;

        //// If you wish to use domain events, then you can add them here:
        //// entity.AddDomainEvent(new SomeDomainEvent(entity));

        //_context.GarageServices.Update(garageEntity);
        //await _context.SaveChangesAsync(cancellationToken);

        //return garageEntity;
    }
}
