using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Conversation;

public partial class Message
{
    [Parameter]
    public string Content { get; set; } = string.Empty;

    [Parameter]
    public string ConversationId { get; set; } = string.Empty;
}