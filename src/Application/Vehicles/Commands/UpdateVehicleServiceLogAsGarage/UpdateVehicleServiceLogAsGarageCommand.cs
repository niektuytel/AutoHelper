using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Common.Mappings;
using AutoHelper.Application.Garages._DTOs;
using AutoHelper.Application.Garages.Commands.CreateGarageItem;
using AutoHelper.Application.Garages.Queries.GetGarageSettings;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLogAsGarage;
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.UpdateVehicleServiceLogAsGarage;

public record UpdateVehicleServiceLogAsGarageCommand : IRequest<VehicleServiceLogAsGarageDtoItem>
{
    public UpdateVehicleServiceLogAsGarageCommand(string userId, UpdateVehicleServiceAsGarageLogDtoItem data)
    {
        UserId = userId;
        ServiceLogId = data.ServiceLogId;
        VehicleLicensePlate = data.VehicleLicensePlate;
        Type = data.Type;
        Description = data.Description;
        Date = data.Date;
        ExpectedNextDate = data.ExpectedNextDate;
        OdometerReading = data.OdometerReading;
        ExpectedNextOdometerReading = data.ExpectedNextOdometerReading;
        Status = data.Status;
    }

    internal string UserId { get; private set; }
    public Guid ServiceLogId { get; private set; }
    public string VehicleLicensePlate { get; set; }
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;
    public string? Description { get; set; }

    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;

    public VehicleServiceLogStatus Status { get; set; }

    public VehicleServiceLogAttachmentDtoItem? Attachment { get; set; }

    internal GarageItem Garage { get; set; }

    internal VehicleServiceLogItem ServiceLog { get; set; } = null!;

    internal DateTime? ParsedDate { get; private set; }

    internal DateTime? ParsedExpectedNextDate { get; private set; }

    public void SetParsedDates(DateTime? date, DateTime? expectedNextDate)
    {
        ParsedDate = date;
        ParsedExpectedNextDate = expectedNextDate;
    }
}

public class UpdateVehicleServiceLogAsGarageCommandHandler : IRequestHandler<UpdateVehicleServiceLogAsGarageCommand, VehicleServiceLogAsGarageDtoItem>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateVehicleServiceLogAsGarageCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IMapper mapper)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogAsGarageDtoItem> Handle(UpdateVehicleServiceLogAsGarageCommand request, CancellationToken cancellationToken)
    {
        var entity = UpdateVehicleServiceLogEntity(request);
        UploadAttachmentIfPresent(request, entity, cancellationToken);

        // update entity
        await _context.SaveChangesAsync(cancellationToken);
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        return _mapper.Map<VehicleServiceLogAsGarageDtoItem>(entity);
    }

    private VehicleServiceLogItem UpdateVehicleServiceLogEntity(UpdateVehicleServiceLogAsGarageCommand request)
    {
        var serviceLog = request.ServiceLog; 
        serviceLog.VehicleLicensePlate = request.VehicleLicensePlate;
        serviceLog.Type = request.Type;
        serviceLog.Description = request.Description;
        serviceLog.Status = request.Status;

        serviceLog.Date = (DateTime)request.ParsedDate!;
        serviceLog.ExpectedNextDate = request.ParsedExpectedNextDate;
        serviceLog.OdometerReading = request.OdometerReading;
        serviceLog.ExpectedNextOdometerReading = request.ExpectedNextOdometerReading;

        return serviceLog;
    }

    private async void UploadAttachmentIfPresent(UpdateVehicleServiceLogAsGarageCommand request, VehicleServiceLogItem entity, CancellationToken cancellationToken)
    {
        if (request.Attachment?.FileName != null && request.Attachment?.FileData != null)
        {
            var fileExtension = Path.GetExtension(request.Attachment.FileName);
            var attachmentBlobName = await _blobStorageService.UploadVehicleAttachmentAsync(
                request.Attachment.FileData,
                fileExtension,
                cancellationToken
            );

            entity.AttachedFile = attachmentBlobName;
        }
    }

}
