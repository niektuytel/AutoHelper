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
using AutoHelper.Domain;
using AutoHelper.Domain.Entities;
using AutoHelper.Domain.Entities.Garages;
using AutoHelper.Domain.Entities.Vehicles;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;

public record CreateVehicleServiceLogDto : IRequest<VehicleServiceLogDtoItem>
{
    public CreateVehicleServiceLogCommand ServiceLogCommand { get; set; }
    public IFormFile AttachmentFile { get; set; }
}

public record CreateVehicleServiceLogCommand : IRequest<VehicleServiceLogDtoItem>
{
    public string VehicleLicensePlate { get; set; }
    public string GarageLookupIdentifier { get; set; }
    public GarageServiceType Type { get; set; } = GarageServiceType.Other;
    public string? Description { get; set; }


    public string Date { get; set; }
    public string? ExpectedNextDate { get; set; } = null!;
    public int OdometerReading { get; set; }
    public int? ExpectedNextOdometerReading { get; set; } = null!;


    public string ReporterName { get; set; } = null!;
    public string? ReporterPhoneNumber { get; set; } = null!;
    public string? ReporterEmailAddress { get; set; } = null!;

    public VehicleServiceLogAttachmentDtoItem Attachment { get; set; }

    [JsonIgnore]
    internal DateTime? ParsedDate { get; private set; }

    [JsonIgnore]
    internal DateTime? ParsedExpectedNextDate { get; private set; }

    public void SetParsedDates(DateTime? date, DateTime? expectedNextDate)
    {
        ParsedDate = date;
        ParsedExpectedNextDate = expectedNextDate;
    }
}

public class CreateVehicleServiceLogCommandHandler : IRequestHandler<CreateVehicleServiceLogCommand, VehicleServiceLogDtoItem>
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateVehicleServiceLogCommandHandler(IBlobStorageService blobStorageService, IApplicationDbContext context, IMapper mapper)
    {
        _blobStorageService = blobStorageService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehicleServiceLogDtoItem> Handle(CreateVehicleServiceLogCommand request, CancellationToken cancellationToken)
    {
        // Align license plate
        request.VehicleLicensePlate = request.VehicleLicensePlate.ToUpper().Replace("-", "");

        var entity = new VehicleServiceLogItem
        {
            VehicleLicensePlate = request.VehicleLicensePlate,
            GarageLookupIdentifier = request.GarageLookupIdentifier,
            Type = request.Type,
            Description = request.Description,
            
            Date = (DateTime)request.ParsedDate!,
            ExpectedNextDate = request.ParsedExpectedNextDate,
            OdometerReading = request.OdometerReading,
            ExpectedNextOdometerReading = request.ExpectedNextOdometerReading,

            Status = VehicleServiceLogStatus.NotVerified,
            ReporterName = request.ReporterName,
            ReporterPhoneNumber = request.ReporterPhoneNumber,
            ReporterEmailAddress = request.ReporterEmailAddress
        };

        // upload attached file
        if(request.Attachment.FileName != null && request.Attachment.FileData != null)
        {
            var fileExtension = Path.GetExtension(request.Attachment.FileName);
            var attachmentBlobName = await _blobStorageService.UploadVehicleAttachmentAsync(
                request.Attachment.FileData,
                fileExtension,
                cancellationToken
            );

            entity.AttachedFile = attachmentBlobName;
        }

        // If you wish to use domain events, then you can add them here:
        // entity.AddDomainEvent(new SomeDomainEvent(entity));

        _context.VehicleServiceLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<VehicleServiceLogDtoItem>(entity);
    }
}
