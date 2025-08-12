using System.Collections.Generic;
using Base;
using Core.Options.Offsets;
using UI.Timers.Controller.Items.Base;

namespace UI.Timers.Presenter
{
    public interface ITimersPresenter : ITimerWidgetsContainer, IVisualAsync, IAlignable, IUnloadable
    {
        bool Active { get; }
        
        void Initialize(IScreenOffsets screenOffsets, IEnumerable<ITimerInfo> infoList);
        void Activate();
        void Deactivate();
        void PlayAlarm();
    }
}