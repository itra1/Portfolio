using System;
using System.Text;

namespace Core.Utils
{
    public static class DateTimeHelper
    {
        public static string GetPeriod(in DateTime from, in DateTime to)
        {
            var buffer = new StringBuilder();
            
            if (from.Year != to.Year)
                buffer.Append($"{from:dd MMMM yyyy}");
            else if (from.Month != to.Month)
                buffer.Append($"{from:dd MMMM}");
            else if (from.Day != to.Day)
                buffer.Append($"{from:dd}");
            
            if (buffer.Length > 0)
                buffer.Append(" - ");
            
            buffer.Append($"{to:dd MMMM yyyy}");
            
            return buffer.ToString();
        }
    }
}