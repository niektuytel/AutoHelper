namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown = 0,
    SucceededMOT = 1,   // used
    FailedMOT = 2,      // used
    Service = 3,
    Repair = 4,
    OwnerChange = 5     // used
}