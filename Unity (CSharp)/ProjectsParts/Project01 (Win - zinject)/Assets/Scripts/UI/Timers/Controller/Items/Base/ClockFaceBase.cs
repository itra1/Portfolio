using System;
using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.UI.Timers.Data;

namespace UI.Timers.Controller.Items.Base
{
    public abstract class ClockFaceBase : TimerBase, ITimer, IAlarmState
    {
        private bool _isAlarmOn;
        private long _endTime;
        
        public bool IsAlarmOn
        {
            get => _isAlarmOn;
            set
            {
                _isAlarmOn = value;
                PrepareToSendInfo();
            }
        }

        public long EndTime
        {
            get
            {
                if (CurrentTime > 0)
                    _endTime = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds() - CurrentTime;
                
                return _endTime;
            }
        }
        
        protected ClockFaceBase(TimerType type,
            IOutgoingStateController outgoingState,
            IOutgoingTimersTickStateController outgoingTimersTickState,
            TimeSpan timeLimit)
            : base(type, outgoingState, outgoingTimersTickState, true, timeLimit) { }

        public override void Stop()
        {
            _endTime = 0L;
            
            base.Stop();
        }

        public override bool Update()
        {
            if (!IsAlarmOn) 
                return base.Update();
            
            var previousTime = CurrentTime;
            
            if (!base.Update())
                return false;
            
            if (previousTime > 0 && CurrentTime <= 0)
                MessageDispatcher.SendMessage(MessageType.TimerPlayAlarm);
            
            return true;
        }
        
        protected override void PrepareToSendInfo()
        {
            var context = OutgoingState.Context;
            
            context.SetTimerInfo(Type, Running, Paused, Visible, IsAlarmOn, EndTime, CurrentTime);
            context.SetTimerPosition(Type, X, Y);
            context.SetTimerColor(Type, Color);
            
            OutgoingState.PrepareToSendIfAllowed();
        }
    }
}