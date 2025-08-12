using System;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Data;

namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick
{
    public partial class OutgoingTimersTickState : OutgoingTimersTickPacket, IOutgoingTimersTickState
    {
        public TimerTickInfo[] Timers = Array.Empty<TimerTickInfo>();
    }
}