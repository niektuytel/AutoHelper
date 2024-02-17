using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Models;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Queries.GetGarageLookup;
using AutoHelper.Application.Garages.Queries.GetGarageLookupCards;
using AutoHelper.Application.Garages.Queries.GetGarageLookups;
using AutoHelper.Application.Garages.Queries.GetGarageServicesAsVehicle;
using AutoHelper.Application.Vehicles.Commands.ReviewVehicleServiceLog;
using Microsoft.AspNetCore.Mvc;

namespace AutoHelper.WebUI.Controllers;

public class GarageController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public GarageController(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    [HttpGet($"{nameof(ServiceLogReview)}")]
    [ProducesResponseType(typeof(IActionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ServiceLogReview([FromQuery] string action)
    {
        var command = new ReviewVehicleServiceLogCommand(action);
        var servicelog = await Mediator.Send(command);
        return Redirect($"/thankyou/{nameof(ServiceLogReview)}");
    }

    [HttpGet($"{nameof(SearchLookups)}/{{licensePlate}}/{{latitude}}/{{longitude}}")]
    [ProducesResponseType(typeof(PaginatedList<GarageLookupBriefDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

        // TODO: Batch locations that is faster search?
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


    [HttpGet($"{nameof(SearchLookupsByName)}")]
    [ProducesResponseType(typeof(GarageLookupDtoItem[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupDtoItem[]> SearchLookupsByName(
        [FromQuery] string name,
        [FromQuery] int maxSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetGarageLookupsByNameQuery(name, maxSize);
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet($"{nameof(SearchLookupCardsByName)}")]
    [ProducesResponseType(typeof(GarageLookupSimplefiedDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupSimplefiedDto[]> SearchLookupCardsByName(
        [FromQuery] string name,
        [FromQuery] int maxSize = 10,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetGarageLookupCardsByNameQuery(name, maxSize);
        return await Mediator.Send(query, cancellationToken);
    }

    [HttpGet($"{nameof(GetLookup)}/{{identifier}}")]
    [ProducesResponseType(typeof(GarageLookupDtoItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<GarageLookupDtoItem> GetLookup([FromRoute] string identifier, [FromQuery] string? licensePlate = null)
    {
        var request = new GetGarageLookupQuery(identifier, licensePlate);
        return await Mediator.Send(request);
    }

    [HttpGet($"{nameof(GetServices)}/{{identifier}}")]
    [ProducesResponseType(typeof(IEnumerable<GarageServiceDtoItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IEnumerable<GarageServiceDtoItem>> GetServices(
        [FromRoute] string identifier,
        [FromQuery] string? licensePlate = null
    )
    {
        var query = new GetGarageServicesAsVehicleQuery(identifier, licensePlate);
        return await Mediator.Send(query);
    }

}
