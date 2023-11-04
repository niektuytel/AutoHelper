namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown,
    SucceededMOT,   // used
    FailedMOT,      // used
    Service,
    Repair,
    OwnerChange     // used
}