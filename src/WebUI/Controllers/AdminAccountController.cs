using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookup;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleLookups;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimeline;
using AutoHelper.Application.Vehicles.Commands.SyncVehicleTimelines;
using AutoHelper.Hangfire.Shared.MediatR;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using WebUI.Models;

namespace AutoHelper.WebUI.Controllers;

[Authorize]
[RequiredScopeOrAppPermission(RequiredScopesConfigurationKey = "AzureAD:Scopes:Admin.ReadWrite")]
public class AdminAccountController: ApiControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public AdminAccountController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
    [HttpPut($"{nameof(SyncGarageLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string SyncGarageLookups(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 100
    )
    {
        var command = new SyncGarageLookupsCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = nameof(SyncGarageLookupsCommand);
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] lookups";

        Mediator.Enqueue(_backgroundJobClient, queue, title, command);
        return $"Successfully start hangfire job: {queue}";
    }

    [HttpPut($"{nameof(SyncVehicleLookup)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> SyncVehicleLookup([FromQuery] string licensePlate)
    {
        var command = new SyncVehicleLookupCommand(licensePlate);
        return await Mediator.Send(command);
    }

    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [HttpPut($"{nameof(SyncVehicleLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string SyncVehicleLookups(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 10000
    )
    {
        var command = new SyncVehicleLookupsCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(SyncVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] lookups";

        Mediator.Enqueue(_backgroundJobClient, queue, title, command);
        return $"Successfully start new queue: {queue}";
    }

    [HttpPut($"{nameof(SyncVehicleTimeline)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> SyncVehicleTimeline([FromQuery] string licensePlate)
    {
        var command = new SyncVehicleTimelineCommand(licensePlate);
        return await Mediator.Send(command);
    }

    /// <param name="endRowIndex">-1 means all of them</param>
    /// <param name="maxInsertAmount">-1 means all of them</param>
    /// <param name="maxUpdateAmount">-1 means all of them</param>
    [HttpPut($"{nameof(SyncVehicleTimelines)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string SyncVehicleTimelines(
        [FromQuery] int startRowIndex = 0,
        [FromQuery] int endRowIndex = -1,
        [FromQuery] int maxInsertAmount = -1,
        [FromQuery] int maxUpdateAmount = 0,
        [FromQuery] int batchSize = 1000
    )
    {
        var command = new SyncVehicleTimelinesCommand(startRowIndex, endRowIndex, maxInsertAmount, maxUpdateAmount, batchSize);
        var queue = $"{nameof(SyncVehicleLookupsCommand)}";
        var title = $"[start:{startRowIndex}/end:{endRowIndex}] max_[insert:{maxInsertAmount}|update:{maxUpdateAmount}] timelines";

        Mediator.Enqueue(_backgroundJobClient, queue, title, command);
        return $"Successfully start queue: {queue}";
    }

}
