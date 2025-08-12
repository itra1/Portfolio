using System;
using UI.Switches.Triggers.Data;

namespace UI.Switches
{
    public interface ITriggerSwitchListener
    {
        bool HasListener(in TriggerKey key, Action<bool> listener);
        bool AddListener(in TriggerKey key, Action<bool> listener);
        bool RemoveListener(in TriggerKey key, Action<bool> listener);
    }
}