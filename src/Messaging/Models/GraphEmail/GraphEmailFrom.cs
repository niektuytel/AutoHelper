using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;
public class GraphEmailFrom
{
    [JsonPropertyName("emailAddress")]
    public GraphEmailAddress EmailAddress { get; set; } = null!;
}