using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;

public class GraphEmailBody
{
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = null!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = null!;
}
