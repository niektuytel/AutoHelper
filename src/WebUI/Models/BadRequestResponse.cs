using Newtonsoft.Json.Linq;

namespace AutoHelper.WebUI.Models;

public class BadRequestResponse
{
    public string type { get; set; }
    public string title { get; set; }
    public int status { get; set; }
    public Dictionary<string, string> errors { get; set; }
}
