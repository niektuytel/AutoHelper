using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageServices;

public record GetGarageServicesQuery : IRequest<IEnumerable<GarageServiceItemDto>>
{
    public GetGarageServicesQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; private set; }
}

public class GetGarageServicesQueryHandler : IRequestHandler<GetGarageServicesQuery, IEnumerable<GarageServiceItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageServicesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GarageServiceItemDto>> Handle(GetGarageServicesQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages.FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        var entities = _context.GarageServices
            .Where(x => 
                x.UserId == request.UserId && 
                x.GarageId == entity.Id
            )
            .AsEnumerable();

        return _mapper.Map<IEnumerable<GarageServiceItemDto>>(entities) ?? new List<GarageServiceItemDto>();
    }

}
