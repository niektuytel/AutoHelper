namespace AutoHelper.Domain.Entities.Vehicles;

public enum VehicleTimelineType
{
    Unknown,
    SucceededMOT,
    FailedMOT,
    Service,
    Repair,
    OwnerChange
}