namespace UI.Timers.Controller.Items.Base
{
    public interface ITimerLaps
    {
        void AddLap();
        void RemoveLapAt(int index);
        void RemoveAllLaps();
    }
}