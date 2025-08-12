using System;
using Base;
using UnityEngine;

namespace UI.Notifications.Presenter.Popups
{
    public interface INotificationPopup : IVisualAsync, INotificationEventDispatcher, IDisposable
    {
        Vector2 Size { get; }
        
        void MoveTo(Vector2 anchoredPosition, float? duration = null);
    }
}