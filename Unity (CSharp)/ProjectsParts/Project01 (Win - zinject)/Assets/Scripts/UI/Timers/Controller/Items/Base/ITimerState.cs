namespace UI.Timers.Controller.Items.Base
{
    public interface ITimerState
    {
        bool Active { get; }
        bool Running { get; }
        bool Paused { get; }
        bool Visible { get; set; }
    }
}