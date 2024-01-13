using System.Text;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.CreateGarageConversationItems;
using AutoHelper.Application.Conversations.Commands.ReceiveEmailMessage;
using AutoHelper.Application.Conversations.Commands.StartConversationItems;
using AutoHelper.Hangfire.MediatR;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace AutoHelper.WebUI.Controllers;

public class ConversationController : ApiControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public ConversationController(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpPost($"{nameof(StartGarageConversation)}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartGarageConversation([FromBody] CreateGarageConversationItemsCommand command, CancellationToken cancellationToken)
    {
        var conversations = await Mediator.Send(command, cancellationToken);
        var conversationIds = conversations.Select(x => x.Id).ToList();

        var queue = nameof(StartConversationItemsCommand);
        var title = $"[{command.UserEmailAddress ?? command.UserWhatsappNumber}]: {command.MessageType.ToString()}";
        var startConversationItemsCommand = new StartConversationItemsCommand(conversationIds);
        Mediator.Enqueue(_backgroundJobClient, queue, title, startConversationItemsCommand);

        return Ok();
    }

    [HttpPost($"{nameof(ReceiveEmailMessage)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(NotFoundResult), StatusCodes.Status404NotFound)]
    public async Task<string> ReceiveEmailMessage([FromBody] ReceiveEmailMessageCommand message, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(message, cancellationToken);
        return result;
    }

}
