using System;
using System.Globalization;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations._DTOs;
using AutoHelper.Application.Vehicles.Commands.CreateVehicleServiceLog;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.CreateGarageConversationItems;

public class CreateGarageConversationItemsValidator : AbstractValidator<CreateGarageConversationItemsCommand>
{
    public CreateGarageConversationItemsValidator(IApplicationDbContext context)
    {
        RuleFor(x => x.UserEmailAddress)
            .Must((command, mail) => !string.IsNullOrEmpty(mail) || !string.IsNullOrEmpty(command.UserWhatsappNumber))
            .WithMessage("Either WhatsApp number or email address is required")
            .EmailAddress()
            .WithMessage("Invalid email address format");

        RuleFor(x => x.UserWhatsappNumber)
            .Must((command, number) => !string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(command.UserEmailAddress))
            .WithMessage("Either WhatsApp number or email address is required")
            .Matches(@"^\+?[0-9]{10,15}$").When(x => !string.IsNullOrEmpty(x.UserWhatsappNumber))
            .WithMessage("Invalid WhatsApp number format");

        RuleFor(x => x.MessageContent)
            .NotEmpty().WithMessage("Message content cannot be empty");

        RuleFor(x => x.Services)
            .NotEmpty().WithMessage("Services collection cannot be empty");

        RuleForEach(x => x.Services)
            .SetValidator(new VehicleServiceValidator(context));
    }
}

public class VehicleServiceValidator : AbstractValidator<VehicleService>
{
    private readonly IApplicationDbContext _context;

    public VehicleServiceValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.GarageServiceId)
            .NotEmpty().WithMessage("Garage service ID is required.")
            .MustAsync(BeValidAndExistingGarageService)
            .WithMessage("Invalid or non-existent garage service.");

        RuleFor(x => x.RelatedGarageLookupIdentifier)
            .NotEmpty().WithMessage("Garage identifier is required.")
            .MustAsync(BeValidAndExistingGarage)
            .WithMessage("Invalid or non-existent garage."); ;

        RuleFor(x => x.RelatedGarageLookupName)
            .NotEmpty().WithMessage("Related garage lookup name is required");

        RuleFor(x => x.ConversationEmailAddress)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.ConversationEmailAddress))
            .WithMessage("Invalid email address format");

        RuleFor(x => x.ConversationWhatsappNumber)
            .Matches(@"^\+?[0-9]{10,15}$").When(x => !string.IsNullOrEmpty(x.ConversationWhatsappNumber))
            .WithMessage("Invalid WhatsApp number format");

        RuleFor(x => x.VehicleLicensePlate)
            .NotEmpty().WithMessage("Vehicle license plate is required.")
            .MustAsync(BeValidAndExistingVehicle)
            .WithMessage("Invalid or non-existent vehicle.");

        RuleFor(x => x.VehicleLongitude)
            .NotEmpty().WithMessage("Vehicle longitude is required")
            .Must(BeAValidLongitude).WithMessage("Invalid longitude format");

        RuleFor(x => x.VehicleLatitude)
            .NotEmpty().WithMessage("Vehicle latitude is required")
            .Must(BeAValidLatitude).WithMessage("Invalid latitude format");
    }

    private async Task<bool> BeValidAndExistingGarageService(VehicleService service, Guid garageServiceId, CancellationToken cancellationToken)
    {
        var entity = await _context.GarageServices
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Id == garageServiceId,
                cancellationToken: cancellationToken
            );

        return entity != null;
    }

    private async Task<bool> BeValidAndExistingGarage(string lookupIdentifier, CancellationToken cancellationToken)
    {
        var garage = await _context.GarageLookups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Identifier == lookupIdentifier, cancellationToken);

        return garage != null;
    }

    private async Task<bool> BeValidAndExistingVehicle(VehicleService service, string licensePlate, CancellationToken cancellationToken)
    {
        licensePlate = licensePlate.ToUpper().Replace("-", "");
        service.VehicleLicensePlate = licensePlate;

        var vehicle = await _context.VehicleLookups
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);

        return vehicle != null;
    }

    private bool BeAValidLongitude(string longitude)
    {
        if (double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lng))
        {
            return lng >= -180 && lng <= 180;
        }
        return false;
    }

    private bool BeAValidLatitude(string latitude)
    {
        if (double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat))
        {
            return lat >= -90 && lat <= 90;
        }
        return false;
    }

}
