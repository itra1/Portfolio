using UnityEngine;

namespace Utils
{
    public static class SizeCalculator
    {
        public static Vector2 Fit(Vector2 parentSize, Vector2 contentSize)
        {
            var parentWidth = parentSize.x;
            var parentHeight = parentSize.y;
            
            var contentWidth = contentSize.x;
            var contentHeight = contentSize.y;
            
            if (parentWidth < contentWidth)
            {
                var factor = parentWidth / contentWidth;
                
                contentWidth *= factor;
                contentHeight *= factor;
            }
            
            if (parentHeight < contentHeight)
            {
                var factor = parentHeight / contentHeight;
                
                contentHeight *= factor;
                contentWidth *= factor;
            }

            return new Vector2(contentWidth, contentHeight);
        }
    }
}