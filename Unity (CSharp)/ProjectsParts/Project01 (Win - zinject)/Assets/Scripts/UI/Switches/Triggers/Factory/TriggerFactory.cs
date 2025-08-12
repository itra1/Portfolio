using System;
using Core.Utils;
using UI.Switches.Triggers.Data.Enums;
using UI.Switches.Triggers.Data.Enums.Attributes;

namespace UI.Switches.Triggers.Factory
{
    public class TriggerFactory : ITriggerFactory
    {
        public ITrigger Create(TriggerType type)
        {
            var attribute = type.GetAttribute<TriggerAttribute>();
            var triggerType = attribute.TriggerType;
            
            if (attribute is not { TriggerType: not null }) 
                return null;
            
            var arguments = attribute.Arguments;
            
            if (arguments is { Length: > 0 }) 
                return (ITrigger) Activator.CreateInstance(triggerType, arguments);
            
            return (ITrigger) Activator.CreateInstance(triggerType);
        }
    }
}