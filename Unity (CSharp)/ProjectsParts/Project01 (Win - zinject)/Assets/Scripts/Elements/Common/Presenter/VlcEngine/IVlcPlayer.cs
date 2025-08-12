using System;
using Base;
using UnityEngine;

namespace Elements.Common.Presenter.VlcEngine
{
    public interface IVlcPlayer : IVlcPlayerState, IDisposable, IVisual, IAlignable, IVlcPlayerEventDispatcher
    {
        void SetParent(RectTransform parent);
        
        void AdjustSize();
        
        void Play();
        void Stop();
        void Refresh();
    }
}