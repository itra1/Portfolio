using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

public static class Iso8601Formats
{
    public static DateTime isoStringToUtc(string str)
    {
        return DateTime.ParseExact(str, "yyyyMMddTHHmmss.fffZ", CultureInfo.InvariantCulture)
            .ToUniversalTime();
    }

    public static List<DateTime> toTimeList(List<string> stringsList)
    {
        List<DateTime> result = new List<DateTime>();
        foreach (var str in stringsList)
        {
            result.Add(isoStringToUtc(str));
        }
        return result;
    }
}
