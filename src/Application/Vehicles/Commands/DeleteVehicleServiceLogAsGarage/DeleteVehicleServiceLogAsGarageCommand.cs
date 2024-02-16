using System.Text.Json.Serialization;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.ReviewVehicleServiceLog;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;

namespace AutoHelper.Application.Vehicles.Commands.DeleteVehicleServiceLogAsGarage;

public class DeleteVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public DeleteVehicleServiceLogAsGarageCommand(string userId, Guid serviceLogId)
    {
        UserId = userId;
        ServiceLogId = serviceLogId;
    }

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    [JsonIgnore]
    public GarageItem Garage { get; set; } = null!;

    public Guid ServiceLogId { get; set; }

    [JsonIgnore]
    public VehicleServiceLogItem ServiceLog { get; set; } = null!;

}

public class DeleteVehicleServiceLogAsGarageCommandHandler : IRequestHandler<DeleteVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public DeleteVehicleServiceLogAsGarageCommandHandler(IMapper mapper, ISender sender)
    {
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(DeleteVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var approveCommand = new ReviewVehicleServiceLogCommand(request.ServiceLog, false);
        await _sender.Send(approveCommand, cancellationToken);

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(request.ServiceLog);
    }
}