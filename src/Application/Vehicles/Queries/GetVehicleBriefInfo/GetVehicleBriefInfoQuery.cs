using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities;
using AutoMapper;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleBriefInfo;

public record GetVehicleBriefInfoQuery : IRequest<VehicleBriefDtoItem>
{
    public GetVehicleBriefInfoQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; private set; }
}

public class GetVehicleBriefInfoQueryHandler : IRequestHandler<GetVehicleBriefInfoQuery, VehicleBriefDtoItem>
{
    private readonly IVehicleService _vehicleService;

    public GetVehicleBriefInfoQueryHandler(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    public async Task<VehicleBriefDtoItem> Handle(GetVehicleBriefInfoQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var info = await _vehicleService.GetVehicleByLicensePlateAsync(request.LicensePlate);
            return info!;
        }
        catch (Exception)
        {
            throw new ValidationException(new List<ValidationFailure>() {
                new(nameof(VehicleBriefDtoItem), $"Search.LicensePlate.NotFound")
            });
        }
    }


}
