using System;
using System.Text.RegularExpressions;
using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AutoHelper.Application.Conversations.Commands.StartConversation
{
    public class StartConversationCommandValidator : AbstractValidator<StartConversationCommand>
    {
        public const string EmailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
        public const string WhatsAppNumberPattern = @"^\+[1-9]{1}[0-9]{3,14}$"; // Simple E.164 format check

        private readonly IApplicationDbContext _context;

        public StartConversationCommandValidator(IApplicationDbContext context)
        {
            _context = context;

            _ = RuleFor(x => x.RelatedGarageLookupId)
                .NotEmpty()
                .WithMessage("RelatedGarageLookupId is required.")
                .MustAsync(BeValidGarage)
                .WithMessage("The provided Garage does not exist.");

            RuleFor(x => x.RelatedVehicleLookupId)
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
                .Must(BeValidEmailOrWhatsApp)
                .WithMessage("SenderWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

            RuleFor(x => x.ReceiverWhatsAppNumberOrEmail)
                .NotEmpty()
                .WithMessage("ReceiverWhatsAppNumberOrEmail is required.")
                .Must(BeValidEmailOrWhatsApp)
                .WithMessage("ReceiverWhatsAppNumberOrEmail should be a valid email or WhatsApp number.");

            RuleFor(x => x.MessageType)
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

        private async Task<bool> BeValidVehicle(StartConversationCommand command, Guid vehicleId, CancellationToken cancellationToken)
        {
            var vehicle = await _context.VehicleLookups.FirstOrDefaultAsync(x => x.Id == vehicleId, cancellationToken);
            if (vehicle != null)
            {
                command.RelatedVehicle = vehicle;
                return true;
            }
            return false;
        }

        private bool BeValidEmailOrWhatsApp(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            if (Regex.IsMatch(input, EmailPattern))
            {
                return true;
            }

            if (Regex.IsMatch(input, WhatsAppNumberPattern))
            {
                return true;
            }

            return false;
        }
    }
}
