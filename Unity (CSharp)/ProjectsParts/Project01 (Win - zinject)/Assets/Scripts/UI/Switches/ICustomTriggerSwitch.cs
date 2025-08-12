using UI.Switches.Triggers.Data;

namespace UI.Switches
{
    public interface ICustomTriggerSwitch : ITriggerSwitchState
    {
        bool Enable(in TriggerKey key);
        bool Disable(in TriggerKey key);
        bool Reset(in TriggerKey key);
    }
}