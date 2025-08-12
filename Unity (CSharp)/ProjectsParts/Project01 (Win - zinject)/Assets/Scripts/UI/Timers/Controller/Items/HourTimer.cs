using System;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.UI.Timers.Attributes;
using Core.UI.Timers.Data;
using UI.Timers.Controller.Items.Base;

namespace UI.Timers.Controller.Items
{
    [Timer(TimerType.Hour)]
    public class HourTimer : ClockFaceBase
    {
        public HourTimer(IOutgoingStateController outgoingState, IOutgoingTimersTickStateController outgoingTimersTickState) 
            : base(TimerType.Hour, outgoingState, outgoingTimersTickState, TimeSpan.FromDays(1)) { }
    }
}