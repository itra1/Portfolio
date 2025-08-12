using UI.Switches.Triggers.Data;

namespace UI.Switches
{
    public interface ITriggerSwitchState
    {
        bool IsOn(in TriggerKey key);
    }
}