using System.Text;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Hangfire.MediatR;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string> StartConversation([FromBody] StartConversationBody conversation, CancellationToken cancellationToken)
    {
        var upsertVehicle = new UpsertVehicleLookupCommand(
            conversation.VehicleLicensePlate,
            conversation.VehicleLatitude,
            conversation.VehicleLongitude,
            conversation.VehiclePhoneNumber,
            conversation.VehicleWhatsappNumber,
            conversation.VehicleEmailAddress
        );
        var vehicleLookup = await Mediator.Send(upsertVehicle, cancellationToken);

        var command = new StartConversationCommand(
            conversation.RelatedGarageLookupId,
            vehicleLookup.Id,
            conversation.RelatedServiceTypes,
            conversation.SenderWhatsAppNumberOrEmail,
            conversation.ReceiverWhatsAppNumberOrEmail,
            conversation.MessageType,
            conversation.MessageContent
        );

        var jobName = this.EnqueueConversation(command);
        return jobName;
    }

    [HttpPost($"{nameof(EnqueueConversation)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string EnqueueConversation([FromBody] StartConversationCommand command)
    {
        var jobName = $"{nameof(StartConversationCommand)}[{command.SenderWhatsAppNumberOrEmail}] >> [{command.ReceiverWhatsAppNumberOrEmail}]";
        Mediator.Enqueue(jobName, command);
        return jobName;
    }

}
