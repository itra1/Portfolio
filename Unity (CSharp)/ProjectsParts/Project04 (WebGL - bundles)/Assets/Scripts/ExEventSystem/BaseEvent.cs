using System;

namespace ExEvent {
    public abstract class BaseEvent
    {
        public static void Call(BaseEvent e) 
        {
            EventDispatcher.Instance.Call(e);
        }

        public static void CallAsync(BaseEvent e)
        {
            EventDispatcher.Instance.CallAsync(e);
        }
    }
}