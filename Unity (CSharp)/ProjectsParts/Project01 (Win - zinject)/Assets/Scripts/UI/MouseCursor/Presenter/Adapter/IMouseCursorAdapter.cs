using UnityEngine;

namespace UI.MouseCursor.Presenter.Adapter
{
    public interface IMouseCursorAdapter
    {
        void SetCursor(Texture2D texture, Vector2 hotspot);
    }
}