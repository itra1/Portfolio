using System;
using UnityEngine;

namespace Settings.Data
{
    [Serializable]
    public struct MouseCursorInfo
    {
        public MouseCursorState State;
        public Texture2D Texture;
        public Vector2 HotSpot;
    }
}