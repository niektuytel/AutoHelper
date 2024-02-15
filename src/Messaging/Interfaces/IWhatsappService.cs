namespace AutoHelper.Messaging.Interfaces;

internal interface IWhatsappService
{
    string GetPhoneNumberId(string phoneNumber);
    string GetValidContent(string content);
}