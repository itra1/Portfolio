using System;
using System.Collections.Generic;
using Core.Network.Socket.Packets.Outgoing.States.TimersTick.Data;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Outgoing.States.TimersTick
{
    public partial class OutgoingTimersTickState
    {
        private readonly IDictionary<string, int> _indicesByName;
        
        public OutgoingTimersTickState() => _indicesByName = new Dictionary<string, int>();
        
        public void AddTimer(string name)
        {
            if (_indicesByName.ContainsKey(name))
            {
                Debug.LogError($"The timer type name \"{name}\" has already been added before");
                return;
            }
            
            var index = Timers.Length;
            
            var info = new TimerTickInfo
            {
                Type = name
            };
            
            _indicesByName.Add(name, index);
            
            Array.Resize(ref Timers, index + 1);
            
            Timers[index] = info;
        }
        
        public void UpdateTimer(string name, long value)
        {
            if (_indicesByName.TryGetValue(name, out var index))
                Timers[index].Value = value;
            else
                Debug.LogError($"Unknown timer type name detected: {name}");
        }
    }
}