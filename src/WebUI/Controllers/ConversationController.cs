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
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Domain.Entities.Conversations;

namespace AutoHelper.WebUI.Controllers;

public class ConversationController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;


    public ConversationController(ICurrentUserService currentUser, IIdentityService identityService)
    {
        _currentUser = currentUser;
        _identityService = identityService;
    }

    [HttpPost($"{nameof(StartConversation)}")]
    [ProducesResponseType(typeof(ConversationItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public ConversationItem StartConversation([FromBody] StartConversationCommand command)
    {
        //var jobId = $"{nameof(StartConversationCommand)}[>>] from({maxInsertAmount}):maxUpdate({maxUpdateAmount})";
        //var command = new UpsertGarageLookupsCommand(maxInsertAmount, maxUpdateAmount);
        //Mediator.Enqueue(jobId, command);

        //return $"Successfully start hangfire job: {jobId}";
        return null;
    }

}
