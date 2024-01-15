using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoHelper.Messaging.Models.GraphEmail;

internal class GraphEmail
{
    [JsonPropertyName("message")]
    public GraphEmailMessage Message { get; set; } = null!;
}
