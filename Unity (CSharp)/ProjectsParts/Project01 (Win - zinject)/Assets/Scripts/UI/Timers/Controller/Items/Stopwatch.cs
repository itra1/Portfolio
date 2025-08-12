using System;
using System.Collections.Generic;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.UI.Timers.Attributes;
using Core.UI.Timers.Data;
using UI.Timers.Controller.Items.Base;

namespace UI.Timers.Controller.Items
{
    [Timer(TimerType.Stopwatch)]
    public class Stopwatch : TimerBase, ITimer, ITimerLaps
    {
        private readonly List<long> _laps;
        
        public Stopwatch(IOutgoingStateController outgoingState, IOutgoingTimersTickStateController outgoingTimersTickState)
            : base(TimerType.Stopwatch, outgoingState, outgoingTimersTickState, false, TimeSpan.FromDays(1))
        {
            _laps = new List<long>();
        }
        
        public void AddLap()
        {
            _laps.Add(CurrentTime);
            PrepareToSendInfo();
        }
        
        public void RemoveLapAt(int index)
        {
            if (index >= _laps.Count) 
                return;
            
            _laps.RemoveAt(index);
            PrepareToSendInfo();
        }

        public void RemoveAllLaps()
        {
            if (_laps.Count == 0) 
                return;
            
            _laps.Clear();
            PrepareToSendInfo();
        }
        
        protected override void PrepareToSendInfo()
        {
            var context = OutgoingState.Context;
            
            context.SetStopwatchInfo(Running, Paused, Visible, StartTime, Paused ? PausedTime : null, CurrentTime, _laps);
            context.SetStopwatchPosition(X, Y);
            context.SetStopwatchColor(Color);
            
            OutgoingState.PrepareToSendIfAllowed();
        }
    }
}