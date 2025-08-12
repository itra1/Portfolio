namespace UI.Timers.Controller.Items.Base
{
    public interface ITimerPosition
    {
        float X { get; }
        float Y { get; }

        void SetPosition(float x, float y);
    }
}