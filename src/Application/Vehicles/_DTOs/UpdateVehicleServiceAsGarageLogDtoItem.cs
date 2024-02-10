using AutoHelper.Domain;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Vehicles._DTOs;

public record UpdateVehicleServiceAsGarageLogDtoItem
{
    public Guid Id { get; set; }
    public string VehicleLicensePlate { get; set; }
    public Guid GarageServiceId { get; set; }
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogStatus Status { get; set; }
    public IFormFile? AttachmentFile { get; set; }
}
