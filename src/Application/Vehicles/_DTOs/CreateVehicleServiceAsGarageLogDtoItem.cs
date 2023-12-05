using AutoHelper.Domain.Entities.Garages;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Vehicles._DTOs;

public record CreateVehicleServiceAsGarageLogDtoItem
{
    public string VehicleLicensePlate { get; set; }

    public Guid? GarageServiceId { get; set; }
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;
    public string? title { get; set; }
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public IFormFile? AttachmentFile { get; set; }
}
