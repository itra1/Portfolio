using UnityEngine;

namespace UI.MouseCursor.Presenter.Adapter
{
    public static class MouseCursorMediator
    {
        private static IMouseCursorAdapter _adapter;
        
        public static void Register(IMouseCursorAdapter adapter) => 
            _adapter = adapter;
        
        public static void SetCursor(Texture2D texture, Vector2 hotspot) => 
            _adapter.SetCursor(texture, hotspot);
        
        public static void Unregister(IMouseCursorAdapter adapter)
        {
            if (_adapter == adapter)
                _adapter = null;
        }
    }
}