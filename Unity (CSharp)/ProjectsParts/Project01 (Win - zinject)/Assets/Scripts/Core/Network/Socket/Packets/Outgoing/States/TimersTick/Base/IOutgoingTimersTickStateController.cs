namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base
{
    public interface IOutgoingTimersTickStateController
    {
        IOutgoingTimersTickStateContext Context { get; }

        void Send();
    }
}