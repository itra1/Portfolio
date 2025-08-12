using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components;
using Elements.Windows.Video.Presenter.Components;
using UnityEngine;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.VideoFile.Components
{
    public class VideoFileHeader : WindowHeader, IAdjustableWidthBar
    {
        private Vector2 _sizeDelta;
        
        public void SaveSizeDelta() => _sizeDelta = RectTransform.sizeDelta;
        
        public void RecalculateSizeDelta(float newWidth)
        {
            var rectTransform = RectTransform;
            var sizeDelta = rectTransform.sizeDelta;
            
            rectTransform.sizeDelta = new Vector2(sizeDelta.x - (rectTransform.rect.size.x - newWidth), sizeDelta.y);
        }
        
        public void RestoreSizeDelta()
        {
            if (_sizeDelta != Vector2.zero)
                RectTransform.sizeDelta = _sizeDelta;
            
            _sizeDelta = Vector2.zero;
        }
    }
}