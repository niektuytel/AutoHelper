namespace AutoHelper.Application.Vehicles._DTOs;


public class VehicleSpecificationsDtoItem
{
    public List<VehicleInfoSectionItem> Data { get; set; } = new List<VehicleInfoSectionItem>();
}

public class VehicleInfoSectionItem
{
    public VehicleInfoSectionItem(string title)
    {
        Title = title;
    }

    public string Title { get; private set; }

    public List<List<string>> Values { get; set; }

}