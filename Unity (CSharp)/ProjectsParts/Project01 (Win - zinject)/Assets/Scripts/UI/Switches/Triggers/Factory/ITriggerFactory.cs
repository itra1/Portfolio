using UI.Switches.Triggers.Data.Enums;

namespace UI.Switches.Triggers.Factory
{
    public interface ITriggerFactory
    {
        ITrigger Create(TriggerType type);
    }
}