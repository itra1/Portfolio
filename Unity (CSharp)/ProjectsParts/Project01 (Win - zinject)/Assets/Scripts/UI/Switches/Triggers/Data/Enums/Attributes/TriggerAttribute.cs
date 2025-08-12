using System;

namespace UI.Switches.Triggers.Data.Enums.Attributes
{
    public class TriggerAttribute : Attribute
    {
        public bool IsStatic { get; }
        public Type TriggerType { get; }
        public object[] Arguments { get; }

        public TriggerAttribute(bool isStatic, Type triggerType, params object[] args)
        {
            IsStatic = isStatic;
            TriggerType = triggerType;
            Arguments = args;
        }
    }
}