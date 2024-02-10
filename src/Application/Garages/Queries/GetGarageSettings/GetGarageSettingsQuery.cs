using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageSettings;

public record GetGarageSettingsQuery : IRequest<GarageSettingsDtoItem>
{
    public GetGarageSettingsQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; set; }
}

public class GetGarageSettingsQueryHandler : IRequestHandler<GetGarageSettingsQuery, GarageSettingsDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageSettingsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageSettingsDtoItem> Handle(GetGarageSettingsQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Garages
            .Include(g => g.Lookup)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (entity == null)
        {
            throw new NotFoundException(nameof(GarageItem), request.UserId);
        }

        return _mapper.Map<GarageSettingsDtoItem>(entity);
    }

}
