using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
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

    [HttpGet($"{{id}}/{nameof(Settings)}")]
    public async Task<GarageSettings> Settings([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageSettingsQuery(id));
    }

    [HttpPost($"{nameof(Create)}")]
    public async Task<Guid> Create([FromBody]CreateGarageItemCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateSettings)}")]
    public async Task<Guid> UpdateSettings([FromBody] UpdateGarageItemSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

}
