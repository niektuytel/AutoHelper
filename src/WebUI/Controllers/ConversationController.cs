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

        var jobName = EnqueueConversation(command);
        return jobName;
    }

    [HttpPost($"{nameof(EnqueueConversation)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string EnqueueConversation([FromBody] StartConversationCommand command)
    {
        var sender = SanitizeForQueueName(command.SenderWhatsAppNumberOrEmail);
        var receiver = SanitizeForQueueName(command.ReceiverWhatsAppNumberOrEmail);
        var jobName = $"{nameof(StartConversationCommand).ToLower()}_{sender}_to_{receiver}";

        Mediator.Enqueue(jobName, command, isRecursive: true);
        return jobName;
    }

    private string SanitizeForQueueName(string input)
    {
        // Only allow lowercase letters, digits, underscores, and dashes
        return new string(input
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-')
            .Select(c => char.IsUpper(c) ? char.ToLower(c) : c)
            .ToArray());
    }

}
