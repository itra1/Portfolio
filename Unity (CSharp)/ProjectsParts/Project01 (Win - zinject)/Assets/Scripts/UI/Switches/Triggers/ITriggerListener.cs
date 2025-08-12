using System;

namespace UI.Switches.Triggers
{
    public interface ITriggerListener
    {
        bool IsEmpty { get; }
        
        bool Contains(Action<bool> listener);
        bool Add(Action<bool> listener);
        bool Remove(Action<bool> listener);
    }
}