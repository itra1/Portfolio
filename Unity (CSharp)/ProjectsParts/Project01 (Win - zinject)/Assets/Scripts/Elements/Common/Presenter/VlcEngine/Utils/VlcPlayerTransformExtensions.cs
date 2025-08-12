using UnityEngine;
using Utils;

namespace Elements.Common.Presenter.VlcEngine.Utils
{
    public static class VlcPlayerTransformExtensions
    {
        public static void ScaleAndCrop(this VlcPlayerBase player, Texture texture)
        {
            var rectTransform = player.RectTransform;
            
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
            
            var rect = rectTransform.rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            var textureWidth = (float) texture.width;
            var textureHeight = (float) texture.height;
            var rectRatio = rectWidth / rectHeight;
            var textureRatio = textureWidth / textureHeight;
            
            if (textureRatio < rectRatio)
                rectTransform.sizeDelta = new Vector2(0f, textureWidth / rectRatio);
            else if (rectRatio < textureRatio)    
                rectTransform.sizeDelta = new Vector2(textureHeight * rectRatio, 0f);
        }
        
        public static void ScaleToFit(this VlcPlayerBase player, Texture texture)
        {
            var rectTransform = player.RectTransform;
            
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
            
            var rect = rectTransform.rect;
            var rectWidth = rect.width;
            var rectHeight = rect.height;
            var textureWidth = (float) texture.width;
            var textureHeight = (float) texture.height;
            var widthRatio = rectWidth / textureWidth;
            var heightRatio = rectHeight / textureHeight;
			
            if (widthRatio < heightRatio)
                rectTransform.sizeDelta = new Vector2(0f, textureHeight * widthRatio - rectHeight);
            else if (heightRatio < widthRatio)
                rectTransform.sizeDelta = new Vector2(textureWidth * heightRatio - rectWidth, 0f);
        }
        
        public static void StretchToFill(this VlcPlayerBase player)
        {
            var rectTransform = player.RectTransform;
            
            rectTransform.ResetAnchors(Vector2.one * 0.5f);
            rectTransform.Reset();
        }
        
        public static void ApplySizeDelta(this VlcPlayerBase player, Vector2 sizeDelta)
        {
            var rectTransform = player.RectTransform;
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            
            rectTransform.sizeDelta = new Vector2(width * sizeDelta.x, height * sizeDelta.y);
        }
    }
}