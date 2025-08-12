using System;
using UnityEngine;

namespace UI.Timers.Presenter.Widgets.Info
{
    [Serializable]
    public struct ActiveColorInfo
    {
        [SerializeField, Range(0f, 1f)] public float Progress;
        [SerializeField] public Color Color;
    }
}