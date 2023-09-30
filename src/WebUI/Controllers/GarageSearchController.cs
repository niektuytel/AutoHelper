using System.Diagnostics.Metrics;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages.Queries.GetGaragesBySearch;
using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleInfo;
using AutoHelper.Application.Vehicles.Queries.GetVehicleServiceLogs;
using AutoHelper.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Extensions;
using WebUI.Models.Response;
using YamlDotNet.Core.Tokens;

namespace AutoHelper.WebUI.Controllers;

public class GarageSearchController : ApiControllerBase
{
    [HttpGet($"{nameof(SearchGarages)}/{{licensePlate}}/{{latitude}}/{{longitude}}")]
    public async Task<PaginatedList<GarageItemSearchDto>> SearchGarages(
        [FromRoute] string licensePlate,
        [FromRoute] float latitude,
        [FromRoute] float longitude,
        [FromQuery] int inKmRange = 10,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    ){
        var query = new GetGaragesBySearchQuery(licensePlate, latitude, longitude, inKmRange, pageNumber, pageSize);
        return await Mediator.Send(query);
    }
    
}
