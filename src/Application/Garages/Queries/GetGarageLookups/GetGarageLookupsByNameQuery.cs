﻿using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages._DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookups;
public class GetGarageLookupsByNameQuery : IRequest<GarageLookupDtoItem[]>
{
    public GetGarageLookupsByNameQuery(string name, int maxSize = 10)
    {
        Name = name;
        MaxSize = maxSize;
    }

    public string Name { get; private set; }
    public int MaxSize { get; private set; }
}

public class GetGaragesByNameQueryHandler : IRequestHandler<GetGarageLookupsByNameQuery, GarageLookupDtoItem[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGaragesByNameQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageLookupDtoItem[]> Handle(GetGarageLookupsByNameQuery request, CancellationToken cancellationToken)
    {
        var garages = await _context.GarageLookups
            .AsNoTracking()
            .Where(g => g.Name.ToLower().Contains(request.Name.ToLower()))
            .Take(request.MaxSize)
            .ToArrayAsync(cancellationToken);

        var entities = _mapper.Map<GarageLookupDtoItem[]>(garages);
        return entities ?? Array.Empty<GarageLookupDtoItem>();
    }
}
