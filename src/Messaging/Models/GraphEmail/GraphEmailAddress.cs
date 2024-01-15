using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;

public class GraphEmailAddress
{
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    public string Name { get; internal set; }
}
