﻿using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageLookupCards;
public class GetGarageLookupCardsByNameQuery : IRequest<GarageLookupSimplefiedDto[]>
{
    public GetGarageLookupCardsByNameQuery(string name, int maxSize = 10)
    {
        Name = name;
        MaxSize = maxSize;
    }

    public string Name { get; private set; }
    public int MaxSize { get; private set; }
}

public class GetGarageLookupCardsByNameQueryHandler : IRequestHandler<GetGarageLookupCardsByNameQuery, GarageLookupSimplefiedDto[]>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageLookupCardsByNameQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageLookupSimplefiedDto[]> Handle(GetGarageLookupCardsByNameQuery request, CancellationToken cancellationToken)
    {
        var garages = await _context.GarageLookups
            .AsNoTracking()
            .Where(g => g.Name.ToLower().Contains(request.Name.ToLower()))
            .Take(request.MaxSize)
            .Select(g => new GarageLookupSimplefiedDto()
            {
                Identifier = g.Identifier,
                Name = g.Name,
                City = g.City,
            })
            .ToArrayAsync(cancellationToken);

        return garages ?? Array.Empty<GarageLookupSimplefiedDto>();
    }
}
