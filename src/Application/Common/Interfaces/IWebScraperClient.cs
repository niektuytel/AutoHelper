
namespace AutoHelper.Infrastructure.Services;

public interface IWebScraperClient
{
    Task<string?> GetEmailAddressAsync(string website);
    Task<string?> GetPhoneNumberAsync(string website);
    Task<string?> GetWhatsappNumberAsync(string website);
}