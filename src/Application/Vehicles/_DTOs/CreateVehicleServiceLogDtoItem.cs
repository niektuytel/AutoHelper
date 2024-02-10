using MediatR;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Vehicles._DTOs;

public record CreateVehicleServiceLogDtoItem : IRequest<VehicleServiceLogDtoItem>
{
    public string VehicleLicensePlate { get; set; }
    public string GarageLookupIdentifier { get; set; }

    public Guid? GarageServiceId { get; set; } = null!;
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public string ReporterName { get; set; } = null!;
    public string? ReporterPhoneNumber { get; set; } = null!;
    public string? ReporterEmailAddress { get; set; } = null!;

    public IFormFile AttachmentFile { get; set; }
}
