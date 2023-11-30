using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Garages.Queries.GetGarageLookupStatus;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimeline;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleTimelines;
using AutoHelper.Hangfire.MediatR;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace AutoHelper.WebUI.Controllers;

public class AdminAccountController: ApiControllerBase
{
    [Authorize(Policy = "Admin")]
    [HttpGet($"{nameof(GetGarageLookupStatuses)}")]
    [ProducesResponseType(typeof(GarageLookupsStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupsStatusDto> GetGarageLookupStatuses(CancellationToken cancellationToken)
    {
        var query = new GetGarageLookupsStatusQuery();
        var response = await Mediator.Send(query, cancellationToken);

        return response;
    }

    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
    [Authorize(Policy = "Admin")]
    [HttpPut($"{nameof(UpsertLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertLookups([FromQuery] int maxInsertAmount = 0, [FromQuery] int maxUpdateAmount = 0)
    {
        var command = new UpsertGarageLookupsCommand(maxInsertAmount, maxUpdateAmount);
        var queue = nameof(UpsertGarageLookupsCommand);
        var title = $"maxInsertAmount: {maxInsertAmount} | maxUpdateAmount: {maxUpdateAmount}";

        Mediator.Enqueue(queue, title, command);
        return $"Successfully start hangfire job: {queue}";
    }



    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [Authorize(Policy = "Admin")]
    [HttpPut($"{nameof(UpsertVehicleLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertVehicleLookups(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 10000
    )
    {
        var command = new UpsertVehicleLookupsCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(UpsertVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] lookups";

        Mediator.Enqueue(queue, title, command);
        return $"Successfully start new queue: {queue}";
    }

    [Authorize(Policy = "Admin")]
    [HttpPut($"{nameof(UpsertVehicleTimeline)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> UpsertVehicleTimeline([FromQuery] string licensePlate)
    {
        var command = new UpsertVehicleTimelineCommand(licensePlate);
        return await Mediator.Send(command);
    }

    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [Authorize(Policy = "Admin")]
    [HttpPut($"{nameof(UpsertVehicleTimelines)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertVehicleTimelines(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 1000
    )
    {
        var command = new UpsertVehicleTimelinesCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(UpsertVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] timelines";

        Mediator.Enqueue(queue, title, command);
        return $"Successfully start queue: {queue}";
    }

}
