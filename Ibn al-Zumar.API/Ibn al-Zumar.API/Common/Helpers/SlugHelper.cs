using System.Text;
using System.Text.RegularExpressions;

namespace IbnAlZumar.Api.Common.Helpers;

public static class SlugHelper
{
    public static string GenerateSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var value = input.Trim().ToLowerInvariant();
        value = Regex.Replace(value, @"[^\p{L}\p{Nd}\s-]", string.Empty);
        value = Regex.Replace(value, @"[\s-]+", "-").Trim('-');

        return value;
    }
}