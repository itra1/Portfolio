using System.Globalization;

namespace Core.Utils
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string original, CultureInfo cultureInfo = null)
        {
            cultureInfo ??= CultureInfo.InvariantCulture;
            return cultureInfo.TextInfo.ToTitleCase(original);
        }
    }
}