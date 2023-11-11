using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Domain.Entities.Conversations.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Messages.Commands.StartConversation
{
    public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
    {
        private const string EmailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        private const string WhatsappPattern = @"^(\+?[1-9]{1}[0-9]{3,14}|[0-9]{9,10})$";


        private readonly IApplicationDbContext _context;

        public StartConversationCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            _ = RuleFor(x => x.RelatedGarageLookupId)
                .NotEmpty()
                .WithMessage("RelatedGarageLookupId is required.")
                .MustAsync(BeValidGarage)
                .WithMessage("The provided Garage does not exist.");

            RuleFor(x => x.RelatedVehicleLicensePlate)
                .NotEmpty()
                .WithMessage("RelatedVehicleLookupId is required.")
                .MustAsync(BeValidVehicle)
                .WithMessage("The provided Vehicle does not exist.");

            RuleFor(x => x.RelatedServiceTypes)
                .NotEmpty()
                .WithMessage("RelatedServiceTypes is required.");

            RuleFor(x => x.SenderWhatsAppNumberOrEmail)
                .NotEmpty()
                .WithMessage("SenderWhatsAppNumberOrEmail is required.")
                .Must(BeValidSenderIdentifier)
                .WithMessage("SenderWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

            RuleFor(x => x.ReceiverWhatsAppNumberOrEmail)
                .NotEmpty()
                .WithMessage("ReceiverWhatsAppNumberOrEmail is required.")
                .Must(BeValidReceiverIdentifier)
                .WithMessage("ReceiverWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

            RuleFor(x => x.ConversationType)
                .NotEmpty()
                .WithMessage("MessageType is required.");

            RuleFor(x => x.MessageContent)
                .NotEmpty()
                .WithMessage("MessageContent is required.");
        }

        private async Task<bool> BeValidGarage(StartConversationCommand command, Guid garageId, CancellationToken cancellationToken)
        {
            var garage = await _context.GarageLookups.FirstOrDefaultAsync(x => x.Id == garageId, cancellationToken);
            if (garage != null)
            {
                command.RelatedGarage = garage;
                return true;
            }
            return false;
        }

        private async Task<bool> BeValidVehicle(StartConversationCommand command, string licensePlate, CancellationToken cancellationToken)
        {
            var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
            if (vehicle != null)
            {
                command.RelatedVehicle = vehicle;
                return true;
            }
            return false;
        }

        private bool BeValidSenderIdentifier(StartConversationCommand command, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            try
            {
                command.SenderContactType = GetContactType(input);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private bool BeValidReceiverIdentifier(StartConversationCommand command, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            try
            {
                command.ReceiverContactType = GetContactType(input);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        private ContactType GetContactType(string senderWhatsAppNumberOrEmail)
        {
            if (Regex.IsMatch(senderWhatsAppNumberOrEmail, EmailPattern))
            {
                return ContactType.Email;
            }
            else if (Regex.IsMatch(senderWhatsAppNumberOrEmail, WhatsappPattern))
            {
                return ContactType.WhatsApp;
            }
            else
            {
                throw new ArgumentException("Invalid sender contact type");
            }
        }
    }
}
