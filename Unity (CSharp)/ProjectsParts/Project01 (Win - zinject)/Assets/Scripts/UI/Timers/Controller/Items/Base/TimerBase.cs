using System;
using Core.Common.Attributes;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Base;
using Core.UI.Timers.Data;
using Core.Utils;

namespace UI.Timers.Controller.Items.Base
{
    public abstract class TimerBase
    {
        private readonly string _name;
        private readonly int _multiplier;
        private readonly long _timeLimit;

        private long _previousTime;
        private bool _visible;
        
        public TimerType Type { get; }
        protected IOutgoingStateController OutgoingState { get; }
        private IOutgoingTimersTickStateController OutgoingTimersTickState { get; }
        
        public bool Active => Running && !Paused;
        public bool Running { get; private set; }
        public bool Paused { get; private set; }
        
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                PrepareToSendInfo();
            }
        }
        
        public long CurrentTime { get; set; }
        public long StartTime { get; set; }
        public long PausedTime { get; set; }
        
        public long TotalTime { get; private set; }
        
        public float X { get; private set; }
        public float Y { get; private set; }

        public string Color { get; private set; }

        protected TimerBase(TimerType type,
            IOutgoingStateController outgoingState,
            IOutgoingTimersTickStateController outgoingTimersTickState,
            bool countdown,
            TimeSpan timeLimit)
        {
            Type = type;
            OutgoingState = outgoingState;
            OutgoingTimersTickState = outgoingTimersTickState;
            
            _name = type.GetAttribute<NameAttribute>()?.Value;
            _multiplier = countdown ? -1 : 1;
            _timeLimit = (long) timeLimit.TotalSeconds;
            
            OutgoingTimersTickState.Context.AddTimer(_name);
        }
        
        public void Start(long time = 0L)
        {
            if (time < 0L)
            {
                Stop();
                return;
            }
            
            if (StartTime == 0L)
                StartTime = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
            
            TotalTime = time;
            CurrentTime = time;
            
            _previousTime = StartTime;
            
            Running = true;
            Paused = false;
            
            PrepareToSendInfo();
            SendTickInfo();
        }
        
        public void Pause()
        {
            if (CurrentTime < 0L)
            {
                Stop();
                return;
            }
            
            Paused = true;
            
            if (PausedTime == 0L)
                PausedTime = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
            
            _previousTime = PausedTime;
            
            PrepareToSendInfo();
        }
        
        public void Resume()
        {
            if (CurrentTime < 0L)
            {
                Stop();
                return;
            }
            
            PausedTime = 0L;
            
            _previousTime = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
            
            Paused = false;
            
            PrepareToSendInfo();
        }
        
        public virtual void Stop()
        {
            Running = false;
            Paused = false;
            
            CurrentTime = 0L;
            TotalTime = 0L;
            StartTime = 0L;
            PausedTime = 0L;
            
            _previousTime = 0L;
            
            PrepareToSendInfo();
            SendTickInfo();
        }
        
        public void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
            
            PrepareToSendInfo();
        }
        
        public void SetColor(string color)
        {
            Color = color;
            
            PrepareToSendInfo();
        }
        
        public virtual bool Update()
        {
            var time = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
            var deltaTime = time - _previousTime;
            
            if (deltaTime < 1L)
                return false;
            
            var currentTime = CurrentTime + _multiplier * deltaTime;
            
            if (currentTime < -_timeLimit || currentTime > _timeLimit)
            {
                Stop();
                return false;
            }
            
            CurrentTime = currentTime;
            
            _previousTime = time;
            
            PrepareToSendInfo();
            SendTickInfo();
            
            return true;
        }
        
        protected abstract void PrepareToSendInfo();

        private void SendTickInfo()
        {
            OutgoingTimersTickState.Context.UpdateTimer(_name, CurrentTime);
            OutgoingTimersTickState.Send();
        }
    }
}