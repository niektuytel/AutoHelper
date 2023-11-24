namespace AutoHelper.Application.Messages._DTOs;

public class VehicleTechnicalDtoItem
{
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int YearOfFirstAdmission { get; set; }
    public string MOTExpiryDate { get; set; }
    public string Mileage { get; set; }
    public string FuelType { get; set; }
    public decimal CombinedFuelConsumption { get; set; }
    public decimal FuelEfficiency { get; set; }
}