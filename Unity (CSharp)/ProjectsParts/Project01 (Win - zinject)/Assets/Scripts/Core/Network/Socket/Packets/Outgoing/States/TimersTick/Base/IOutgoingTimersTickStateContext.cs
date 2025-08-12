namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base
{
    public interface IOutgoingTimersTickStateContext
    {
        void AddTimer(string name);
        void UpdateTimer(string name, long value);
    }
}