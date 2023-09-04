using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Models;
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace AutoHelper.WebUI.Controllers;

[Authorize]
public class GarageController : ApiControllerBase
{
    [HttpGet($"{{id}}/{nameof(Overview)}")]
    public async Task<GarageOverview> Overview([FromRoute]Guid id)
    {
        return await Mediator.Send(new GetGarageOverviewQuery(id));
    }

    [HttpGet($"{{id}}/{nameof(Services)}")]
    public async Task<IEnumerable<GarageServiceItem>> Services([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageServicesQuery(id));
    }

    [HttpGet($"{{id}}/{nameof(Settings)}")]
    public async Task<GarageSettings> Settings([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageSettingsQuery(id));
    }

    [HttpPut($"{nameof(UpdateSettings)}")]
    public async Task<GarageSettings> UpdateSettings([FromBody] UpdateGarageItemSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(Create)}")]
    public async Task<GarageSettings> Create([FromBody]CreateGarageItemCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateService)}")]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceItemCommand command)
    {
        return await Mediator.Send(command);
    }
}
