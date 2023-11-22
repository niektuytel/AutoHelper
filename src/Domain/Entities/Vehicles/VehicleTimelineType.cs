namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown = 0,
    SucceededMOT = 1,   // used
    FailedMOT = 2,      // used
    Service = 3,        // used
    Repair = 4,         // used
    OwnerChange = 5     // used
}