using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace AutoHelper.Infrastructure.Services;
internal class WebScraperClient
{
    private readonly HtmlWeb _web;

    public WebScraperClient(HtmlWeb web)
    {
        _web = web;
    }

    public async Task<string?> GetPhoneNumberAsync(string website)
    {
        return await GetAttributeValueAsync(website, "//a[contains(@href, 'tel')]");
    }

    public async Task<string?> GetEmailAddressAsync(string website)
    {
        var email = await GetAttributeValueAsync(website, "//a[contains(@href, 'mailto')]");
        if (!string.IsNullOrWhiteSpace(email))
        {
            return email.Replace("mailto:", "").Trim();
        }

        const string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";
        var match = Regex.Match(await GetHtmlContentAsync(website), emailPattern);

        return match.Success ? match.Value : null;
    }

    public async Task<string?> GetWhatsappNumberAsync(string website)
    {
        // Step 1: Try to fetch directly from the href attribute
        var number = await GetAttributeValueAsync(website, "//a[contains(@href, 'whatsapp')]");
        if (!string.IsNullOrWhiteSpace(number))
        {
            return number.Replace("whatsapp:", "").Trim();
        }

        // Step 2: Try to find a pattern in the HTML content
        const string whatsappPattern = @"(?:\+1|\+44|\+91|\+316|06)\d{7,9}";
        var match = Regex.Match(await GetHtmlContentAsync(website), whatsappPattern);

        return match.Success ? match.Value : null;
    }

    private async Task<HtmlDocument> LoadDocumentAsync(string website)
    {
        return await _web.LoadFromWebAsync(website);
    }

    private async Task<string?> GetAttributeValueAsync(string website, string xpath)
    {
        var doc = await LoadDocumentAsync(website);
        var node = doc.DocumentNode.SelectSingleNode(xpath);
        return node?.GetAttributeValue("href", null);
    }

    private async Task<string> GetHtmlContentAsync(string website)
    {
        var doc = await LoadDocumentAsync(website);
        return doc.DocumentNode.OuterHtml;
    }
}

