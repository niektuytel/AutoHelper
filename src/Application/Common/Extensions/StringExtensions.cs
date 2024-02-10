using System.Globalization;
using System.Text.RegularExpressions;

namespace AutoHelper.Application.Common.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string str)
    {
        // First, remove any non-alphanumeric characters and convert to lower case
        string cleanStr = Regex.Replace(str, "[^a-zA-Z0-9]", " ").ToLower();

        // Split the string into words
        string[] words = cleanStr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        // Capitalize the first letter of each word
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(words[i]);
            }
        }

        // Combine the words back into a single string, with spaces
        return string.Join(" ", words);
    }
}
