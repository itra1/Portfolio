using UnityEngine;

namespace Utils.Data
{
    public struct RectTransformSnapshot
    {
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector2 OffsetMin;
        public Vector2 OffsetMax;
        public Vector2 SizeDelta;
        public Vector2 AnchoredPosition;
        public Vector3 LocalPosition;
        public Vector3 LocalRotation;
        public Vector3 LocalScale;
    }
}