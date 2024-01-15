using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;

public class GraphEmailMessage
{
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = null!;

    [JsonPropertyName("body")]
    public GraphEmailBody Body { get; set; } = null!;

    [JsonPropertyName("toRecipients")]
    public GraphEmailRecipient[] ToRecipients { get; set; } = null!;
    public GraphEmailAddress From { get; internal set; }
}
