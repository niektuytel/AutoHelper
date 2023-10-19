using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Domain.Entities.Conversations;
using AutoHelper.Domain.Entities.Vehicles;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Vehicles._DTOs;

public class VehicleLookupDtoItem : IMapFrom<VehicleLookupItem>
{
    public VehicleLookupDtoItem()
    { }

    public Guid Id { get; set; }

    public string LicensePlate { get; set; }

    public DateTime MOTExpiryDate { get; set; }

    public Geometry Location { get; set; }

    public string? PhoneNumber { get; set; }

    public string? WhatsappNumber { get; set; }

    public string? EmailAddress { get; set; }

}
