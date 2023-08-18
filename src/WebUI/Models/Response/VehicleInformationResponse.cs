namespace WebUI.Models.Response;

public class VehicleInformationResponse
{
    public List<List<string>> CardInfo { get; set; } = new();

    public List<VehicleInformationSection> Data { get; set; } = new List<VehicleInformationSection>();
}

public class VehicleInformationSection
{
    public VehicleInformationSection(string title)
    {
        Title = title;
    }

    public string Title { get; private set; }

    public List<List<string>> Values { get; set; }

}
