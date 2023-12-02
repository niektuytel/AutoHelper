using AutoHelper.Domain;
using AutoHelper.Domain.Entities.Garages;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Vehicles._DTOs;

public record UpdateVehicleServiceAsGarageLogDto
{
    public Guid ServiceLogId { get; set; }
    public string VehicleLicensePlate { get; set; }
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogStatus Status { get; set; }

    public IFormFile AttachmentFile { get; set; }
}
