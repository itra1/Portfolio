using UnityEngine;

namespace Utils
{
    public static class RectExtensions
    {
        public static Rect Intersect(this Rect first, Rect second)
        {
            var x = Mathf.Max(first.x, second.x);
            var num1 = Mathf.Min(first.x + first.width, second.x + second.width);
            var y = Mathf.Max(first.y, second.y);
            var num2 = Mathf.Min(first.y + first.height, second.y + second.height);
            return num1 >= x && num2 >= y ? new Rect(x, y, num1 - x, num2 - y) : Rect.zero;
        }
    }
}