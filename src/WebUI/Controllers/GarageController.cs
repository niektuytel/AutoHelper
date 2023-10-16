using System.Security.Claims;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Security;
using AutoHelper.Application.Garages.Commands.CreateGarageEmployee;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Commands.CreateGarageServiceItem;
using AutoHelper.Application.Garages.Commands.DeleteGarageEmployee;
using AutoHelper.Application.Garages.Commands.DeleteGarageService;
using AutoHelper.Application.Garages.Commands.UpdateGarageEmployee;
using AutoHelper.Application.Garages.Commands.UpdateGarageItemSettings;
using AutoHelper.Application.Garages.Commands.UpdateGarageService;
using AutoHelper.Application.Garages.Queries.GetGarageEmployees;
using AutoHelper.Application.Garages.Queries.GetGarageOverview;
using AutoHelper.Application.Garages.Queries.GetGaragesLookups;
using AutoHelper.Application.Garages.Queries.GetGarageServices;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.TodoItems.Commands.CreateTodoItem;
using AutoHelper.Application.Vehicles.Queries;
using AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;
using AutoHelper.Application.WeatherForecasts.Queries.GetWeatherForecasts;
using AutoHelper.Domain.Entities.Garages;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models.Response;
using AutoHelper.Application.Garages.Queries.GetGarageLookup;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages.Queries.GetGarageServiceTypesByLicensePlate;
using AutoHelper.Application.Garages.Commands.UpsertGarageLookups;
using AutoHelper.Application.Garages.Queries.GetGarageLookupStatus;
using AutoHelper.Hangfire.MediatR;
using Hangfire.Server;

namespace AutoHelper.WebUI.Controllers;

public class GarageController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;


    public GarageController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    [HttpGet($"{nameof(GetServiceTypesByLicensePlate)}/{{licensePlate}}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceType>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceType>> GetServiceTypesByLicensePlate([FromRoute] string licensePlate)
    {
        var query = new GetGarageServiceTypesByLicensePlateQuery(licensePlate);
        return await Mediator.Send(query);
    }

    [HttpGet($"{nameof(SearchLookups)}/{{licensePlate}}/{{latitude}}/{{longitude}}")]
    [ProducesResponseType(typeof(PaginatedList<GarageLookupBriefDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<PaginatedList<GarageLookupBriefDto>> SearchLookups(
        [FromRoute] string licensePlate,
        [FromRoute] float latitude,
        [FromRoute] float longitude,
        CancellationToken cancellationToken,
        [FromQuery] int inMetersRange = 5000,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? autoCompleteOnGarageName = null,
        [FromQuery] string[]? filters = null
    )
    {
        var query = new GetGarageLookupsQuery(
            licensePlate,
            latitude,
            longitude,
            inMetersRange,
            autoCompleteOnGarageName,
            filters,
            pageNumber,
            pageSize
        );

        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet($"{nameof(GetLookup)}/{{identifier}}")]
    [ProducesResponseType(typeof(GarageLookupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupDto> GetLookup([FromRoute] string identifier, [FromQuery] string licensePlate)
    {
        var request = new GetGarageLookupQuery(identifier, licensePlate);
        return await Mediator.Send(request);
    }

    [Authorize]// TODO: (Policy="Admin")
    [HttpGet($"{nameof(GetLookupsStatus)}")]
    [ProducesResponseType(typeof(GarageLookupsStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupsStatusDto> GetLookupsStatus(CancellationToken cancellationToken)
    {
        var query = new GetGarageLookupsStatusQuery();
        var response = await Mediator.Send(query, cancellationToken);

        return response;
    }

    /// <param name="maxInsertAmount">-1 is all of them</param>
    /// <param name="maxUpdateAmount">-1 is all of them</param>
    [Authorize]// TODO: (Policy="Admin")
    [HttpPut($"{nameof(UpsertLookups)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string UpsertLookups([FromQuery] int maxInsertAmount=0, [FromQuery] int maxUpdateAmount=0)
    {
        var jobId = $"{nameof(UpsertGarageLookupsCommand)}:maxInsert({maxInsertAmount}):maxUpdate({maxUpdateAmount})";
        var command = new UpsertGarageLookupsCommand(maxInsertAmount, maxUpdateAmount);
        Mediator.Enqueue(jobId, command);

        return $"Successfully start hangfire job: {jobId}";
    }

}
