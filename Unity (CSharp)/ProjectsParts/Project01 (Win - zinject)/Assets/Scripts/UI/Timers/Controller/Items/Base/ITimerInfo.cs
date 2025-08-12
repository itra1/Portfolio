namespace UI.Timers.Controller.Items.Base
{
    public interface ITimerInfo : ITimerState, ITimerType, ITimerPosition, ITimerColor
    {
        long CurrentTime { get; }
        long TotalTime { get; }
    }
}