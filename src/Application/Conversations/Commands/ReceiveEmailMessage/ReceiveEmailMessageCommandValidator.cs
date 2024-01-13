using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Extensions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Conversations.Commands.StartConversationItems;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.ReceiveEmailMessage;

public class ReceiveEmailMessageCommandValidator : AbstractValidator<ReceiveEmailMessageCommand>
{

    private readonly IApplicationDbContext _context;

    public ReceiveEmailMessageCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        //_ = RuleFor(x => x.RelatedGarageLookupIdentifier)
        //    .NotEmpty()
        //    .WithMessage("RelatedGarageLookupId is required.")
        //    .MustAsync(BeValidGarage)
        //    .WithMessage("The provided Garage does not exist.");

        //RuleFor(x => x.RelatedVehicleLicensePlate)
        //    .NotEmpty()
        //    .WithMessage("RelatedVehicleLookupId is required.")
        //    .MustAsync(BeValidVehicle)
        //    .WithMessage("The provided Vehicle does not exist.");

        //RuleFor(x => x.RelatedServiceIds)
        //    .NotEmpty()
        //    .WithMessage("RelatedServiceTypes is required.");

        //RuleFor(x => x.SenderWhatsAppNumberOrEmail)
        //    .NotEmpty()
        //    .WithMessage("SenderWhatsAppNumberOrEmail is required.")
        //    .Must(BeValidSenderIdentifier)
        //    .WithMessage("SenderWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

        //RuleFor(x => x.ReceiverWhatsAppNumberOrEmail)
        //    .NotEmpty()
        //    .WithMessage("ReceiverWhatsAppNumberOrEmail is required.")
        //    .Must(BeValidReceiverIdentifier)
        //    .WithMessage("ReceiverWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

        //RuleFor(x => x.ConversationType)
        //    .NotEmpty()
        //    .WithMessage("MessageType is required.");

        //RuleFor(x => x.MessageContent)
        //    .NotEmpty()
        //    .WithMessage("MessageContent is required.");
    }

    //private async Task<bool> BeValidGarage(ReceiveEmailMessageCommand command, string garageIdentifier, CancellationToken cancellationToken)
    //{
    //    var garage = await _context.GarageLookups.FirstOrDefaultAsync(x => x.Identifier == garageIdentifier, cancellationToken);
    //    if (garage != null)
    //    {
    //        command.RelatedGarage = garage;
    //        return true;
    //    }
    //    return false;
    //}

    //private async Task<bool> BeValidVehicle(ReceiveEmailMessageCommand command, string licensePlate, CancellationToken cancellationToken)
    //{
    //    var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
    //    if (vehicle != null)
    //    {
    //        command.RelatedVehicle = vehicle;
    //        return true;
    //    }
    //    return false;
    //}

    //private bool BeValidSenderIdentifier(ReceiveEmailMessageCommand command, string input)
    //{
    //    if (string.IsNullOrWhiteSpace(input))
    //    {
    //        return false;
    //    }

    //    try
    //    {
    //        command.SenderContactType = input.GetContactType();
    //        return true;
    //    }
    //    catch (Exception)
    //    {

    //        return false;
    //    }
    //}

    //private bool BeValidReceiverIdentifier(ReceiveEmailMessageCommand command, string input)
    //{
    //    if (string.IsNullOrWhiteSpace(input))
    //    {
    //        return false;
    //    }

    //    try
    //    {
    //        command.ReceiverContactType = input.GetContactType();
    //        return true;
    //    }
    //    catch (Exception)
    //    {

    //        return false;
    //    }
    //}

    //private ContactType GetContactType(string senderWhatsAppNumberOrEmail)
    //{
    //    if (Regex.IsMatch(senderWhatsAppNumberOrEmail, EmailPattern))
    //    {
    //        return ContactType.Email;
    //    }
    //    else if (Regex.IsMatch(senderWhatsAppNumberOrEmail, WhatsappPattern))
    //    {
    //        return ContactType.WhatsApp;
    //    }
    //    else
    //    {
    //        throw new ArgumentException("Invalid sender contact type");
    //    }
    //}
}
