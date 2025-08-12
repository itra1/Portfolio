using Base;

namespace UI.Switches.Triggers
{
    public interface ICustomTrigger : ITrigger, IResettable
    {
        void Enable();
        void Disable();
    }
}