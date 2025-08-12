using Base;
using UI.Timers.Controller.Items.Base;

namespace UI.Timers.Presenter.Widgets
{
    public interface ITimerWidget : ITimerType, IVisual, IVisible
    {
        ITimerInfo Info { get; }
        
        void Initialize(ITimerInfo info);
        void Refresh();
    }
}