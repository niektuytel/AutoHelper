using FluentValidation;

namespace AutoHelper.Application.Vehicles.Commands.CreateVehicleLookup
{
    public class CreateVehicleLookupCommandValidator : AbstractValidator<CreateVehicleLookupCommand>
    {
        public CreateVehicleLookupCommandValidator()
        {
            // LicensePlate validation
            RuleFor(v => v.LicensePlate)
                .NotEmpty().WithMessage("LicensePlate is required.")
                .MaximumLength(10).WithMessage("LicensePlate must not exceed 10 characters.");

            // MOTExpiryDate validation
            RuleFor(v => v.MOTExpiryDate)
                .NotEmpty().WithMessage("MOTExpiryDate is required.");

            // Location validation
            RuleFor(v => v.Latitude)
                .NotEmpty()
                .WithMessage("Location Latitude is required.");
            RuleFor(v => v.Longitude)
                .NotEmpty()
                .WithMessage("Location Longitude is required.");

            // PhoneNumber validation (optional, but if provided should follow some pattern)
            RuleFor(v => v.PhoneNumber)
                .MaximumLength(15).WithMessage("PhoneNumber must not exceed 15 characters.")
                .When(v => !string.IsNullOrEmpty(v.PhoneNumber)); // Only validate when PhoneNumber is provided

            // WhatsappNumber validation (similar to PhoneNumber)
            RuleFor(v => v.WhatsappNumber)
                .MaximumLength(15).WithMessage("WhatsappNumber must not exceed 15 characters.")
                .When(v => !string.IsNullOrEmpty(v.WhatsappNumber));

            // EmailAddress validation
            RuleFor(v => v.EmailAddress)
                .EmailAddress().WithMessage("Invalid Email Address format.")
                .When(v => !string.IsNullOrEmpty(v.EmailAddress));
        }
    }
}

