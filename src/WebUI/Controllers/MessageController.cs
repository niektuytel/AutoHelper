﻿using System.Text;
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
                .Where(item => item.VehicleLicensePlate == vehicle.LicensePlate);

            foreach (var garage in garages)
            {
                var services = selectedServices.Services
                    .Where(item => 
                        item.VehicleLicensePlate == vehicle.LicensePlate && 
                        item.RelatedGarageLookupIdentifier == garage.RelatedGarageLookupIdentifier
                    )
                    .Select(item => item.RelatedServiceType)
                    .ToArray();

                var command = new StartConversationCommand(
                    garage.RelatedGarageLookupIdentifier,
                    vehicleLookup.LicensePlate,
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
        var queue = nameof(StartConversationCommand);
        var title = $"{command.SenderWhatsAppNumberOrEmail.ToLower()} to {command.ReceiverWhatsAppNumberOrEmail.ToLower()} about {command.RelatedVehicle.LicensePlate}";

        Mediator.Enqueue(queue, title, command, isRecursive: true);
        return queue;
    }
}
