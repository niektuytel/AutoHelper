using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Mvc;

namespace AutoHelper.WebUI.Controllers;

public class GarageController : ApiControllerBase
{
    [HttpGet($"{{id}}/{nameof(Overview)}")]
    public async Task<GarageOverview> Overview([FromRoute]Guid id)
    {
        return await Mediator.Send(new GetGarageOverviewQuery(id));
    }

    [HttpPost($"{{id}}/{nameof(Settings)}")]
    public async Task<GarageSettings> Settings([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetGarageSettingsQuery(id));
    }
}
