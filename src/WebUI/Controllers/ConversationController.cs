using System.Text;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.StartConversation;
using AutoHelper.Hangfire.MediatR;
using AutoHelper.WebUI.Models;
using GoogleApi.Entities.Search.Common;
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

    [HttpPost($"{nameof(StartConversations)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public async Task<string[]> StartConversations([FromBody] SelectedServices selectedServices, CancellationToken cancellationToken)
    {
        var vehicles = selectedServices.Services
            .Select(item => new { 
                LicensePlate = item.VehicleLicensePlate,
                Latitude = item.VehicleLatitude,
                Longitude = item.VehicleLongitude
            })
            .Distinct();

        string[] jobNames = { "" };
        foreach (var vehicle in vehicles)
        {
            var garages = selectedServices.Services
                .Where(item => item.VehicleLicensePlate == vehicle.LicensePlate);

            foreach (var garage in garages)
            {
                //var services = selectedServices.Services
                //    .Where(item => 
                //        item.VehicleLicensePlate == vehicle.LicensePlate && 
                //        item.RelatedGarageLookupIdentifier == garage.RelatedGarageLookupIdentifier
                //    )
                //    .Select(item => item.RelatedServiceType)
                //    .ToArray();

                throw new NotImplementedException("Missing service of garage");

                var command = new StartConversationCommand(
                    garage.RelatedGarageLookupIdentifier,
                    vehicle.LicensePlate,
                    null,
                    selectedServices.SenderPhoneNumber,
                    garage.GarageContactIdentifier,
                    selectedServices.MessageType,
                    selectedServices.MessageContent
                );

                var jobName = EnqueueConversation(command);
                jobNames.Append(jobName);
            }
        }


        return jobNames;
    }

    [HttpPost($"{nameof(EnqueueConversation)}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BadRequestResponse), StatusCodes.Status400BadRequest)]
    public string EnqueueConversation([FromBody] StartConversationCommand command)
    {
        var queue = nameof(StartConversationCommand);
        var title = $"{command.SenderWhatsAppNumberOrEmail.ToLower()} to {command.ReceiverWhatsAppNumberOrEmail.ToLower()} about {command.RelatedVehicle.LicensePlate}";

        Mediator.Enqueue(queue, title, command, isRecursive: true);
        return queue;
    }
}
