namespace AutoHelper.Application.Garages.Queries.GetGarageLookupStatus;

public class GarageLookupsStatusDto
{
    public int AbleToInsert { get; set; }
    public int AbleToUpdate { get; set; }
    public int UpToDate { get; set; }
    public int Total { get; set; }
}

