using Settings.Data;
using UI.MouseCursor.Presenter.Components;

namespace UI.MouseCursor.Presenter
{
    public interface IMouseCursorAccess
    {
        MouseCursorState CurrentState { get; }
        
        void Set(in MouseCursorState state, IMouseCursorUpdater initiator);
        void Remove(IMouseCursorUpdater initiator);
    }
}