using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates.Conversation;

public partial class MessageConfirmation
{
    [Parameter]
    public string ConversationId { get; set; } = string.Empty;
}