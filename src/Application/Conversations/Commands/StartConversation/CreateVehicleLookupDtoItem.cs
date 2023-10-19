using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Vehicles;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Conversations.Commands.StartConversation;

public class CreateVehicleLookupDtoItem
{
    public CreateVehicleLookupDtoItem()
    { }

    public string LicensePlate { get; set; }

    public string Longitude { get; set; }

    public string Latitude { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }
}
