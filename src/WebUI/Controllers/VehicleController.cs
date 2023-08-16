using AutoHelper.Application.TodoLists.Commands.CreateTodoList;
using AutoHelper.Application.TodoLists.Commands.DeleteTodoList;
using AutoHelper.Application.TodoLists.Commands.UpdateTodoList;
using AutoHelper.Application.TodoLists.Queries.ExportTodos;
using AutoHelper.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WebUI.Models.Response;

namespace AutoHelper.WebUI.Controllers;

public class VehicleController : ApiControllerBase
{
    [HttpGet("search")]
    public async Task<ActionResult<LicencePlateBriefResponse>> SearchVehicle([FromQuery] string licencePlate)
    {
        licencePlate = licencePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/5xwu-cdq3.json?kenteken={licencePlate}";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JArray.Parse(json);
        if (data.HasValues)
        {
            var value = data.First().Value<string>("kenteken");
            var rsponse200 = new LicencePlateBriefResponse(value);
            return Ok(rsponse200);
        }

        var response404 = new LicencePlateBriefResponse(licencePlate);
        return NotFound(response404);
    }

    [HttpGet("overview")]
    public async Task<ActionResult<LicencePlateBriefResponse>> GetVehicleOverview([FromQuery] string licencePlate)
    {
        licencePlate = licencePlate.Replace("-", "").ToUpper();
        var url = $"https://opendata.rdw.nl/resource/5xwu-cdq3.json?kenteken={licencePlate}&limit=1";

        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-App-Token", "OKPXTphw9Jujrm9kFGTqrTg3x");
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var data = JArray.Parse(json);
        if (data.HasValues)
        {
            var value = data.First().Value<string>("kenteken");
            var rsponse200 = new LicencePlateBriefResponse(value);
            return Ok(rsponse200);
        }

        var response404 = new LicencePlateBriefResponse(licencePlate);
        return NotFound(response404);
    }

}
