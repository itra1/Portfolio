using Core.UI.Timers.Data;

namespace UI.Timers.Presenter
{
    public interface ITimerWidgetsContainer
    {
        bool IsAnyWidgetVisible { get; }
        
        bool ShowWidget(in TimerType type);
        bool HideWidget(in TimerType type);
    }
}