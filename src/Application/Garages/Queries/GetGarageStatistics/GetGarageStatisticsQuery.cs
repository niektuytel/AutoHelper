using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Domain.Entities.Garages;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageStatistics;

public record GetGarageStatisticsQuery : IRequest<GarageStatisticsDtoItem>
{
    public GetGarageStatisticsQuery(string userId)
    {
        UserId = userId;
    }

    [JsonIgnore]
    public string UserId { get; private set; }

    [JsonIgnore]
    public GarageItem? Garage { get; set; } = new GarageItem();

}

public class GetGarageOverviewQueryHandler : IRequestHandler<GetGarageStatisticsQuery, GarageStatisticsDtoItem>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageOverviewQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<GarageStatisticsDtoItem> Handle(GetGarageStatisticsQuery request, CancellationToken cancellationToken)
    {
        var statistics = new GarageStatisticsDtoItem();


        return statistics;
    }

}
