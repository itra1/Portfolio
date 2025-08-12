using System;
using UnityEngine;

namespace Core.Utils
{
    public static class ColorHelper
    {
        public static Color Parse(string value)
        {
            if (value.Contains('#'))
                value = value[1..];
            
            var r = Convert.ToInt32(value.Substring(0, 2), 16);
            var g = Convert.ToInt32(value.Substring(2, 2), 16);
            var b = Convert.ToInt32(value.Substring(4, 2), 16);
            
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }
}