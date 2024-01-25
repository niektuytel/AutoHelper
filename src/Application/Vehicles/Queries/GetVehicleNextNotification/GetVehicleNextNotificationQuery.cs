using System.Text.Json.Serialization;
using AutoHelper.Application.Common.Exceptions;
using AutoHelper.Application.Common.Interfaces;
using AutoHelper.Application.Vehicles._DTOs;
using AutoHelper.Application.Vehicles.Queries.GetVehicleSpecificationsCard;
using AutoHelper.Domain.Entities.Messages.Enums;
using AutoHelper.Domain.Entities.Vehicles;
using MediatR;

namespace AutoHelper.Application.Vehicles.Queries.GetVehicleNextNotification;

public record GetVehicleNextNotificationQuery : IRequest<VehicleNextNotificationItem>
{
    public GetVehicleNextNotificationQuery(string licensePlate)
    {
        LicensePlate = licensePlate;
    }

    public string LicensePlate { get; set; }

    [JsonIgnore]
    public VehicleLookupItem? Vehicle { get; set; } = null!;
}

public class GetVehicleNextNotificationQueryHandler : IRequestHandler<GetVehicleNextNotificationQuery, VehicleNextNotificationItem>
{
    public GetVehicleNextNotificationQueryHandler()
    {
    }

    public async Task<VehicleNextNotificationItem> Handle(GetVehicleNextNotificationQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var triggerDate = now.AddYears(1);
        var notificationType = VehicleNotificationType.Other;

        // MOT Expiry
        var motTriggerDate = GetMOTTriggerDate(request.Vehicle!);
        if (motTriggerDate > now && motTriggerDate < triggerDate)
        {
            triggerDate = motTriggerDate;
            notificationType = VehicleNotificationType.MOT;
        }

        // Service Logs, oil change on each 10.000 km/1 year after winter
        var winterServiceTriggerDate = GetWinterServiceTriggerDate(now);
        if (winterServiceTriggerDate > now && winterServiceTriggerDate < triggerDate)
        {
            triggerDate = winterServiceTriggerDate;
            notificationType = VehicleNotificationType.WinterService;
        }

        // Prepare tyres for the summer
        var summerTyreChangeTriggerDate = GetSummerTyreChangeTriggerDate(now);
        if (summerTyreChangeTriggerDate > now && summerTyreChangeTriggerDate < triggerDate)
        {
            triggerDate = summerTyreChangeTriggerDate;
            notificationType = VehicleNotificationType.ChangeToSummerTyre;
        }

        // Prepare for going on holiday
        var summerCheckTriggerDate = GetSummerCheckTriggerDate(now);
        if (summerCheckTriggerDate > now && summerCheckTriggerDate < triggerDate)
        {
            triggerDate = summerCheckTriggerDate;
            notificationType = VehicleNotificationType.SummerCheck;
        }

        // Prepare tyres for the winter
        var winterTyreChangeTriggerDate = GetWinterTyreChangeTriggerDate(now);
        if (winterTyreChangeTriggerDate > now && winterTyreChangeTriggerDate < triggerDate)
        {
            triggerDate = winterTyreChangeTriggerDate;
            notificationType = VehicleNotificationType.ChangeToWinterTyre;
        }

        // Service Logs, oil change on each 10.000 km/1 year after summer
        var summerServiceTriggerDate = GetSummerServiceTriggerDate(now);
        if (summerServiceTriggerDate > now && summerServiceTriggerDate < triggerDate)
        {
            triggerDate = summerServiceTriggerDate;
            notificationType = VehicleNotificationType.SummerService;
        }

        var entity = new VehicleNextNotificationItem
        {
            TriggerDate = triggerDate,
            NotificationType = notificationType
        };

        return entity;
    }

    /// <summary>
    /// 1th mot: after 4 years
    /// 2th mot: after 6 years
    /// 3th mot: after 8 years
    /// rest mot: every year
    /// 
    /// https://www.seniorweb.nl/tip/apk-herinneringsbrief-via-berichtenbox-mijnoverheid (6 weeks before, they send a notification)
    /// So we will send it 4 weeks before the MOT date
    private static DateTime GetMOTTriggerDate(VehicleLookupItem vehicle)
    {
        var motDate = vehicle.DateOfMOTExpiry!.Value;

        // 7 days * 4 = 4 weeks
        return motDate.AddDays(-(7 * 4));
    }

    /// <summary>
    /// After the winter vacation, we will send a notification to change the oil
    /// As most users drive more in the winter
    /// 
    /// we will send a notification in February(5th)
    /// </summary>
    private static DateTime GetWinterServiceTriggerDate(DateTime now)
    {
        // Create a DateTime object representing June 15st of the current year
        var februaryThisYear = new DateTime(now.Year, 2, 5);

        // Check if the current date is before feb 5st
        if (now < februaryThisYear)
        {
            return februaryThisYear;
        }
        else
        {
            return februaryThisYear.AddYears(1);
        }
    }

    /// <summary>
    /// Most cars change their winter tyres to summer tyres in April,
    /// We will send a notification in the beginning of April
    /// </summary>
    private static DateTime GetSummerTyreChangeTriggerDate(DateTime now)
    {
        // Create a DateTime object representing April 1st of the current year
        var aprilThisYear = new DateTime(now.Year, 4, 1);

        // Check if the current date is before April 1st
        if (now < aprilThisYear)
        {
            return aprilThisYear;
        }
        else
        {
            return aprilThisYear.AddYears(1);
        }
    }

    /// <summary>
    /// Most cars change their summer tyres to winter tyres in October/November,
    /// We will send a notification on the end of October
    /// </summary>
    private static DateTime GetWinterTyreChangeTriggerDate(DateTime now)
    {
        // Create a DateTime object representing October 25st of the current year
        var octoberThisYear = new DateTime(now.Year, 10, 25);

        // Check if the current date is before October 25st
        if (now < octoberThisYear)
        {
            return octoberThisYear;
        }
        else
        {
            return octoberThisYear.AddYears(1);
        }
    }

    /// <summary>
    /// Most people go on holiday in the summer, 
    /// so we will send a notification in the middle of June
    /// </summary>
    private static DateTime GetSummerCheckTriggerDate(DateTime now)
    {
        // Create a DateTime object representing June 15st of the current year
        var juneThisYear = new DateTime(now.Year, 6, 15);

        // Check if the current date is before june 15st
        if (now < juneThisYear)
        {
            return juneThisYear;
        }
        else
        {
            return juneThisYear.AddYears(1);
        }
    }

    /// <summary>
    /// After the summer vacation, we will send a notification to change the oil
    /// As most users go on holiday in the summer
    /// 
    /// we will send a notification in September(5th)
    /// </summary>
    private static DateTime GetSummerServiceTriggerDate(DateTime now)
    {
        // Create a DateTime object representing September 15st of the current year
        var septemberThisYear = new DateTime(now.Year, 9, 5);

        // Check if the current date is before September 5st
        if (now < septemberThisYear)
        {
            return septemberThisYear;
        }
        else
        {
            return septemberThisYear.AddYears(1);
        }
    }

}
