using System.Text;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Messages.Commands.StartConversation;
using AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup;
using AutoHelper.Hangfire.MediatR;
using AutoHelper.WebUI.Models;
using GoogleApi.Entities.Search.Common;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace AutoHelper.WebUI.Controllers;

public class MessageController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;
    private readonly IIdentityService _identityService;


    public MessageController(ICurrentUserService currentUser, IIdentityService identityService)
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
            var upsertVehicle = new UpsertVehicleLookupCommand(
                vehicle.LicensePlate,
                vehicle.Latitude,
                vehicle.Longitude,
                selectedServices.SenderPhoneNumber,
                selectedServices.SenderWhatsappNumber,
                selectedServices.SenderEmailAddress
            );

            var vehicleLookup = await Mediator.Send(upsertVehicle, cancellationToken);

            var garages = selectedServices.Services
                .Where(item => item.VehicleLicensePlate == vehicle.LicensePlate)
                .DistinctBy(x => x.RelatedGarageLookupId);

            foreach (var garage in garages)
            {
                var services = selectedServices.Services
                    .Where(item => 
                        item.VehicleLicensePlate == vehicle.LicensePlate && 
                        item.RelatedGarageLookupId == garage.RelatedGarageLookupId
                    )
                    .Select(item => item.RelatedServiceType)
                    .Distinct()
                    .ToArray();

                var command = new StartConversationCommand(
                    garage.RelatedGarageLookupId,
                    vehicleLookup.Id,
                    services,
                    selectedServices.SenderPhoneNumber,
                    selectedServices.SenderWhatsappNumber,
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
        var sender = SanitizeForQueueName(command.SenderWhatsAppNumberOrEmail);
        var receiver = SanitizeForQueueName(command.ReceiverWhatsAppNumberOrEmail);
        var jobName = $"{nameof(StartConversationCommand).ToLower()}_{sender}_to_{receiver}";

        Mediator.Enqueue(jobName, command, isRecursive: true);
        return jobName;
    }

    private static string SanitizeForQueueName(string input)
    {
        // Only allow lowercase letters, digits, underscores, and dashes
        return new string(input
            .Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-')
            .Select(c => char.IsUpper(c) ? char.ToLower(c) : c)
            .ToArray());
    }

}
