using System.Text.Json.Serialization;

namespace AutoHelper.Messaging.Models.GraphEmail;

public class GraphEmailRecipient
{
    [JsonPropertyName("emailAddress")]
    public GraphEmailAddress EmailAddress { get; set; } = null!;
}
