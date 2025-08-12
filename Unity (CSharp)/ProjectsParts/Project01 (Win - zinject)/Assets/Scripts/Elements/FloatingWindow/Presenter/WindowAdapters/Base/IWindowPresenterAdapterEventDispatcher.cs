using System;

namespace Elements.FloatingWindow.Presenter.WindowAdapters.Base
{
    public interface IWindowPresenterAdapterEventDispatcher
    {
        event Action StateChanged;
        event Action DragStarted;
        event Action Drag;
        event Action DragStopped;
        event Action Closed;
    }
}