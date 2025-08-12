using Settings.Data;
using UnityEngine.EventSystems;

namespace UI.MouseCursor.Presenter.Components
{
    public class ResizableMouseCursorUpdater : MouseCursorUpdaterBase,
        IPointerEnterHandler, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler, IPointerExitHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }
		
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }
		
        public virtual void OnPointerMove(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }
		
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }
        
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Remove(this);
        }
		
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            MouseCursorAccess.Set(MouseCursorState.Resize, this);
        }
		
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (IsMouseCursorHover(eventData))
                MouseCursorAccess.Set(MouseCursorState.Resize, this);
            else
                MouseCursorAccess.Remove(this);
        }
    }
}