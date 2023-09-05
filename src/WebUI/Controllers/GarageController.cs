using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Commands.UpdateGarageService;
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
    [HttpGet($"{{id}}/{nameof(GetOverview)}")]
    public async Task<GarageOverview> GetOverview([FromRoute]Guid id)
    {
        return await Mediator.Send(new GetGarageOverviewQuery(id));
    }

    [HttpGet($"{{id}}/{nameof(GetServices)}")]
    public async Task<IEnumerable<GarageServiceItem>> GetServices([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageServicesQuery(id));
    }

    [HttpGet($"{{id}}/{nameof(GetSettings)}")]
    public async Task<GarageSettings> GetSettings([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageSettingsQuery(id));
    }

    [HttpPut($"{nameof(UpdateSettings)}")]
    public async Task<GarageSettings> UpdateSettings([FromBody] UpdateGarageSettingsCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut($"{nameof(UpdateService)}")]
    public async Task<GarageServiceItem> UpdateService([FromBody] UpdateGarageServiceCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateGarage)}")]
    public async Task<GarageSettings> CreateGarage([FromBody]CreateGarageCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost($"{nameof(CreateService)}")]
    public async Task<GarageServiceItem> CreateService([FromBody] CreateGarageServiceCommand command)
    {
        return await Mediator.Send(command);
    }
}
