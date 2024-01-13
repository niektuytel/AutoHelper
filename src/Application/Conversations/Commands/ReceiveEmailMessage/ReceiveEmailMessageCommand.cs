using System.Net.Mail;
using MediatR;

namespace AutoHelper.Application.Conversations.Commands.ReceiveEmailMessage;

public class ReceiveEmailMessageCommand : IRequest<string>
{
    public string From { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}

public class ReceiveEmailMessageCommandHandler : IRequestHandler<ReceiveEmailMessageCommand, string>
{
    public Task<string> Handle(ReceiveEmailMessageCommand request, CancellationToken cancellationToken)
    {
        //EmailMessage
        // TODO: Validate message:
        // valid conversation ID?
        // - then add message to conversation
        // - create response message(with, to, cc, subject, content)
        // - then return 200 OK with response message
        // otherwise throw 404 error





        //var vehicles = selectedServices.Services
        //    .Select(item => new {
        //        LicensePlate = item.VehicleLicensePlate,
        //        Latitude = item.VehicleLatitude,
        //        Longitude = item.VehicleLongitude
        //    })
        //    .Distinct();

        //string[] jobNames = { "" };
        //foreach (var vehicle in vehicles)
        //{
        //    var garages = selectedServices.Services
        //        .Where(item => item.VehicleLicensePlate == vehicle.LicensePlate);

        //    foreach (var garage in garages)
        //    {
        //        //var services = selectedServices.Services
        //        //    .Where(item => 
        //        //        item.VehicleLicensePlate == vehicle.LicensePlate && 
        //        //        item.RelatedGarageLookupIdentifier == garage.RelatedGarageLookupIdentifier
        //        //    )
        //        //    .Select(item => item.RelatedServiceType)
        //        //    .ToArray();

        //        throw new NotImplementedException("Missing service of garage");

        //        var command = new StartConversationCommand(
        //            garage.RelatedGarageLookupIdentifier,
        //            vehicle.LicensePlate,
        //            null,
        //            selectedServices.SenderPhoneNumber,
        //            garage.GarageContactIdentifier,
        //            selectedServices.MessageType,
        //            selectedServices.MessageContent
        //        );

        //        var jobName = EnqueueConversation(command);
        //        jobNames.Append(jobName);
        //    }
        //}


        //return jobNames;

        //return Ok();
        throw new NotImplementedException();
    }
}
