using System;

namespace UI.Switches.Triggers
{
    public interface ITrigger : ITriggerState, ITriggerListener, IDisposable
    {
        bool Charged { get; }

        void Fire();
    }
}