using System;
using Core.UI.Timers.Data;

namespace Core.UI.Timers.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TimerAttribute : Attribute
    {
        public TimerType Type { get; }
        
        public TimerAttribute(TimerType type)
        {
            Type = type;
        }
    }
}