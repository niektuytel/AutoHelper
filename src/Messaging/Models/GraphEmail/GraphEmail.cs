using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;

internal class GraphEmail
{
    [JsonPropertyName("message")]
    public GraphEmailMessage Message { get; set; } = null!;
}
