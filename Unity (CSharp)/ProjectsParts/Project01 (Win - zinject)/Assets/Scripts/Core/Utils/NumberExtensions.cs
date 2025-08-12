using System.Globalization;

namespace Core.Utils
{
    public static class NumberExtensions
    {
        private static readonly CultureInfo _culture = new ("ru-RU");
        
        public static string Separate<TNumber>(this TNumber source) where TNumber : struct
            => string.Format(_culture, "{0:N0}", source).Trim();
        
        public static string Separate<TNumber>(this TNumber? source) where TNumber : struct
            => string.Format(_culture, "{0:N0}", source == null ? 0 : (TNumber) source).Trim();
        
        public static string TrimFraction<TNumber>(this TNumber source, int digitsAfterDot) where TNumber : struct
            => string.Format(_culture, $"{{0:F{digitsAfterDot}}}", source).Trim();
        
        public static string TrimFraction<TNumber>(this TNumber? source, int digitsAfterDot) where TNumber : struct
            => string.Format(_culture, $"{{0:F{digitsAfterDot}}}", source == null ? 0 : (TNumber) source).Trim();
    }
}