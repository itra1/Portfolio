using UnityEngine;
using Utils.Data;

namespace Utils
{
    public static class RectTransformExtensions
    {
        public static bool IsAncestorFor(this RectTransform rectTransform, Transform target)
        {
            if (target == null || target == rectTransform)
                return false;
            
            var root = target.root;
            
            if (target == root)
                return false;
            
            do
            {
                var parent = target.parent;
                
                if (parent == null)
                    break;
                
                if (parent == rectTransform)
                    return true;
                
                if (parent == root)
                    break;
                
                target = parent;
            } 
            while (true);
            
            return false;
        }
        
        public static void ResetAnchors(this RectTransform rectTransform, Vector2 pivot)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = pivot;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        public static void ResetAnchors(this RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = pivot;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
        
        public static void Reset(this RectTransform rectTransform)
        {
            rectTransform.localPosition = Vector2.zero;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.localRotation = Quaternion.identity;
            rectTransform.localScale = Vector3.one;
        }
        
        public static void SaveTo(this RectTransform rectTransform, out RectTransformSnapshot snapshot)
        {
            snapshot = new RectTransformSnapshot
            {
                AnchorMin = rectTransform.anchorMin,
                AnchorMax = rectTransform.anchorMax,
                Pivot = rectTransform.pivot,
                OffsetMin = rectTransform.offsetMin,
                OffsetMax = rectTransform.offsetMax,
                SizeDelta = rectTransform.sizeDelta,
                AnchoredPosition = rectTransform.anchoredPosition,
                LocalPosition = rectTransform.localPosition,
                LocalRotation = rectTransform.localRotation.eulerAngles,
                LocalScale = rectTransform.localScale
            };
        }
        
        public static RectTransformSnapshot MakeSnapshot(this RectTransform rectTransform)
        {
            rectTransform.SaveTo(out var snapshot);
            return snapshot;
        }
        
        public static void RestoreFrom(this RectTransform rectTransform, in RectTransformSnapshot snapshot)
        {
            rectTransform.anchorMin = snapshot.AnchorMin;
            rectTransform.anchorMax = snapshot.AnchorMax;
            rectTransform.pivot = snapshot.Pivot;
            rectTransform.offsetMin = snapshot.OffsetMin;
            rectTransform.offsetMax = snapshot.OffsetMax;
            rectTransform.sizeDelta = snapshot.SizeDelta;
            rectTransform.anchoredPosition = snapshot.AnchoredPosition;
            rectTransform.localPosition = snapshot.LocalPosition;
            rectTransform.localRotation = Quaternion.Euler(snapshot.LocalRotation);
            rectTransform.localScale = snapshot.LocalScale;
        }
    }
}