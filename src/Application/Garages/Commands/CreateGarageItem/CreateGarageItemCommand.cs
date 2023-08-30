using System.ComponentModel.DataAnnotations;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Deprecated;
using AutoHelper.Domain.Events;
using MediatR;

namespace AutoHelper.Application.Garages.Commands.CreateGarageItem;

public record CreateGarageItemCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public LocationItem Location { get; set; }

    public BusinessOwnerItem BusinessOwner { get; set; }

    public BankingInfoItem BankingDetails { get; set; }
}

public class CreateGarageItemCommandHandler : IRequestHandler<CreateGarageItemCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateGarageItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateGarageItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new GarageItem
        {
            Id = request.Id,
            Name = request.Name,
            Location = request.Location,
            BusinessOwner = request.BusinessOwner,
            BankingDetails = request.BankingDetails
        };

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.Garages.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
