using System;

namespace Base.Presenter
{
    public interface IEventDispatcher
    {
        event EventHandler Shown;
        event EventHandler Hidden;
        event EventHandler Unloaded;
    }
}