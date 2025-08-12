namespace UI.Timers.Controller.Items.Base
{
    public interface ITimer : ITimerInfo
    {
        void Start(long time = 0L);
        void Pause();
        void Resume();
        void Stop();
        bool Update();
    }
}