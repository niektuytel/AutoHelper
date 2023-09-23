using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Garages.Queries.GetGarageEmployees;

public record GetGarageEmployeesQuery : IRequest<IEnumerable<GarageEmployeeItemDto>>
{
    public GetGarageEmployeesQuery(string userId)
    {
        UserId = userId;
    }

    public string UserId { get; private set; }

}

public class GetGarageEmployeesQueryHandler : IRequestHandler<GetGarageEmployeesQuery, IEnumerable<GarageEmployeeItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetGarageEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GarageEmployeeItemDto>> Handle(GetGarageEmployeesQuery request, CancellationToken cancellationToken)
    {
        var entities = _context.GarageEmployees
            .Where(x => x.UserId == request.UserId)
            .Include(x => x.Contact)
            .Include(x => x.WorkSchema)
            .Include(x => x.WorkExperiences)
            .AsEnumerable();

        return _mapper.Map<IEnumerable<GarageEmployeeItemDto>>(entities) ?? new List<GarageEmployeeItemDto>();
    }

}
