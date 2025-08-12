using System;
using Base;
using UI.MouseCursor.Presenter.Components;
using UnityEngine.EventSystems;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Common.Components
{
    public class GrabArea : DraggableMouseCursorUpdater, IVisible, IVisual
    {
        public bool Visible => gameObject.activeSelf;
        
        public event Action DragStarted;
        public event Action Drag;
        public event Action DragStopped;
        
        public bool Show()
        {
            if (Visible)
                return false;

            gameObject.SetActive(true);
            return true;
        }

        public bool Hide()
        {
            if (!Visible)
                return false;

            gameObject.SetActive(false);
            return true;
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            DragStarted?.Invoke();
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            DragStarted?.Invoke();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            Drag?.Invoke();
        }
        
        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            DragStopped?.Invoke();
        }
        
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            DragStopped?.Invoke();
        }
    }
}