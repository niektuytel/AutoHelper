using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Threading.Tasks;
using global::Microsoft.AspNetCore.Components;

namespace AutoHelper.Messaging.Templates;

public partial class MessageConfirmation
{
    [Parameter]
    public string ConversationId { get; set; } = string.Empty;
}