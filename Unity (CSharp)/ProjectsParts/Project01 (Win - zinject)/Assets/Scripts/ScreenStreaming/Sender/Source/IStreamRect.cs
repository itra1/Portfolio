using UnityEngine;

namespace ScreenStreaming.Sender.Source
{
    public interface IStreamRect
    {
        void SetStreamRect(RectTransform rectTransform);
        void UpdateStreamRect(RectTransform rectTransform);
        void RemoveStreamRect(RectTransform rectTransform);
    }
}