using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using Microsoft.AspNetCore.Mvc;

namespace AutoHelper.WebUI.Controllers;

public class GarageController : ApiControllerBase
{
    [HttpGet($"{nameof(Overview)}")]
    public async Task<IEnumerable<WeatherForecast>> Overview()
    {
        return await Mediator.Send(new GetWeatherForecastsQuery());
    }

    [HttpPost($"{nameof(Settings)}")]
    public async Task<IEnumerable<WeatherForecast>> Settings()
    {
        return await Mediator.Send(new GetWeatherForecastsQuery());
    }
}
