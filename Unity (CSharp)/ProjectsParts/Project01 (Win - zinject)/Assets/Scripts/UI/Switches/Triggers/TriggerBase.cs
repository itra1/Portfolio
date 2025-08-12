using System;
using System.Collections.Generic;

namespace UI.Switches.Triggers
{
    public abstract class TriggerBase
    {
        private readonly ISet<Action<bool>> _listeners;
        
        public bool Enabled { get; protected set; }
        public virtual bool Charged { get; protected set; }
        
        public bool IsEmpty => _listeners.Count == 0;
        
        protected TriggerBase(bool enabled)
        {
            _listeners = new HashSet<Action<bool>>();
            Enabled = enabled;
        }
        
        public bool Contains(Action<bool> listener) => _listeners.Contains(listener);
        public bool Add(Action<bool> listener) => _listeners.Add(listener);
        public bool Remove(Action<bool> listener) => _listeners.Remove(listener);
        
        public void Fire()
        {
            Charged = false;
            Enabled = !Enabled;
            
            foreach (var listener in _listeners)
                listener.Invoke(Enabled);
        }
        
        public void Dispose() => _listeners.Clear();
    }
}