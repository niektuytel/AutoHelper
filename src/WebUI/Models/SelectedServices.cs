using AutoHelper.Domain.Entities.Conversations.Enums;
using AutoHelper.Domain.Entities.Garages;

namespace AutoHelper.WebUI.Models;

public class SelectedServices
{
    public string? SenderPhoneNumber { get; set; }

    public string? SenderWhatsappNumber { get; set; }

    public string? SenderEmailAddress { get; set; }

    public ConversationType MessageType { get; set; }

    public string MessageContent { get; set; }

    public IEnumerable<SelectedService> Services { get; set; }

}