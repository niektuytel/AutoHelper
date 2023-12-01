using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AutoHelper.Application.Vehicles._DTOs;

public record CreateVehicleServiceLogDto : IRequest<VehicleServiceLogDtoItem>
{
    public CreateVehicleServiceLogCommand ServiceLogCommand { get; set; }
    public IFormFile AttachmentFile { get; set; }
}
