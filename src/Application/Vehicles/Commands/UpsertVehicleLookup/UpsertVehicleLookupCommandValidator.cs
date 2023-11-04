using System.Drawing;
using System.Globalization;
using AutoHelper.Application.Common.Interfaces;
using FluentValidation;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace AutoHelper.Application.Vehicles.Commands.UpsertVehicleLookup
{
    public class UpsertVehicleLookupCommandValidator : AbstractValidator<UpsertVehicleLookupCommand>
    {
        public UpsertVehicleLookupCommandValidator(IVehicleService vehicleInfoService)
        {
            RuleFor(v => v.LicensePlate)
                .NotEmpty().WithMessage("LicensePlate is required.")
                .MaximumLength(10).WithMessage("LicensePlate must not exceed 10 characters.");

            RuleFor(v => v.Latitude)
                .NotEmpty().WithMessage("Location Latitude is required.")
                .Must(lat => float.TryParse(lat, out _)).WithMessage("Invalid Latitude format.");

            RuleFor(v => v.Longitude)
                .NotEmpty().WithMessage("Location Longitude is required.")
                .Must(lon => float.TryParse(lon, out _)).WithMessage("Invalid Longitude format.");

            RuleFor(v => v.PhoneNumber)
                .MaximumLength(15).WithMessage("PhoneNumber must not exceed 15 characters.")
                .When(v => !string.IsNullOrEmpty(v.PhoneNumber));

            RuleFor(v => v.WhatsappNumber)
                .MaximumLength(15).WithMessage("WhatsappNumber must not exceed 15 characters.")
                .When(v => !string.IsNullOrEmpty(v.WhatsappNumber));

            RuleFor(v => v.EmailAddress)
                .EmailAddress().WithMessage("Invalid Email Address format.")
                .When(v => !string.IsNullOrEmpty(v.EmailAddress));

            RuleFor(v => v)
                .Must(HaveAtLeastOneContactMethod)
                .WithMessage("At least one of Email, PhoneNumber, or WhatsappNumber must be set.");

            RuleFor(v => v)
                .Must(HaveValidLocation)
                .WithMessage("Invalid Latitude or Longitude format.");

            RuleFor(v => v.LicensePlate)
                .NotEmpty().WithMessage("LicensePlate is required.")
                .CustomAsync(async (licensePlate, context, cancellationToken) => 
                {
                    var cmd = (UpsertVehicleLookupCommand)context.InstanceToValidate;
                    var vehicleInfo = await vehicleInfoService.GetVehicleByLicensePlateAsync(licensePlate);

                    // Store vehicleInfo in the validation context for reuse
                    context.RootContextData["VehicleInfo"] = vehicleInfo;

                    if (vehicleInfo == null)
                    {
                        context.AddFailure("Invalid License Plate");
                        return;
                    }

                    if (!DateTime.TryParse(vehicleInfo.MOTExpiryDate, out var motExpiryDate))
                    {
                        context.AddFailure("Invalid MOT Expiry Date");
                        return;
                    }

                    cmd.MOTExpiryDate = motExpiryDate;
                });

        }

        private bool HaveValidLocation(UpsertVehicleLookupCommand command)
        {
            if(
                double.TryParse(command.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double lat) && 
                double.TryParse(command.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out double lon)
            ) {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                command.Location = geometryFactory.CreatePoint(new Coordinate(lon, lat));
                
                return true;
            }

            command.Location = null!;
            return false;
        }
        private bool HaveAtLeastOneContactMethod(UpsertVehicleLookupCommand command)
        {
            return !string.IsNullOrEmpty(command.EmailAddress) ||
                   !string.IsNullOrEmpty(command.PhoneNumber) ||
                   !string.IsNullOrEmpty(command.WhatsappNumber);
        }
    }
}

