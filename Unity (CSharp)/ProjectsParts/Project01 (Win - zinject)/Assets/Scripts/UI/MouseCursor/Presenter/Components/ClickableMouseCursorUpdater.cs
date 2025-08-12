using Settings.Data;
using UnityEngine.EventSystems;

namespace UI.MouseCursor.Presenter.Components
{
    public class ClickableMouseCursorUpdater : MouseCursorUpdaterBase,
        IPointerEnterHandler, IPointerDownHandler, IPointerMoveHandler, IPointerUpHandler, IPointerExitHandler
    {
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Pointer, this);
        }
		
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Pointer, this);
        }
		
        public virtual void OnPointerMove(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Set(MouseCursorState.Pointer, this);
        }
		
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (!eventData.dragging)
            {
                if (IsMouseCursorHover(eventData))
                    MouseCursorAccess.Set(MouseCursorState.Pointer, this);
                else
                    MouseCursorAccess.Remove(this);
            }
        }
		
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.dragging)
                MouseCursorAccess.Remove(this);
        }
    }
}