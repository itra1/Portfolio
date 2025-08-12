using System;
using System.Collections.Generic;
using Core.Base;
using Core.UI.Timers.Data;
using UI.Timers.Controller.Items.Base;

namespace UI.Timers.Controller.Items.Factory
{
    public interface ITimersFactory : ILateInitialized, IDisposable
    {
        IReadOnlyList<TimerType> Types { get; }
        
        ITimer Create(TimerType type);
    }
}