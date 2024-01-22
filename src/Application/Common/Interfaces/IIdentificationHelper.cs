namespace AutoHelper.Application.Common.Interfaces;

public interface IIdentificationHelper
{
    string GetValidIdentifier(string? emailAddress, string? whatsappNumber);
    string GetPhoneNumberId(string phoneNumber);
}