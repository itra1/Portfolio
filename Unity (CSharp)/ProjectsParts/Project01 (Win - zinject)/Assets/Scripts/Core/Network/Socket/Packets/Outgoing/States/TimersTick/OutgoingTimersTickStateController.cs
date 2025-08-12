using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;

namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick
{
    public class OutgoingTimersTickStateController : IOutgoingTimersTickStateController
    {
        private readonly IOutgoingTimersTickState _outgoingState;
        
        public IOutgoingTimersTickStateContext Context => _outgoingState;
        
        public OutgoingTimersTickStateController(IOutgoingTimersTickState outgoingState) =>
            _outgoingState = outgoingState;
        
        public void Send() => _outgoingState.Send();
    }
}