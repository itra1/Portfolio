using System;

namespace Elements.Common.Presenter.VlcEngine
{
    public interface IVlcPlayerEventDispatcher
    {
        event Action Playing;
        event Action Stopped;
        event Action Displayed;
        event Action Disposed;
    }
}