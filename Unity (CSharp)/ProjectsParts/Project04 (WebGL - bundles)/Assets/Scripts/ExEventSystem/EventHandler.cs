using System;

namespace ExEvent {
    public class ExEventHandler : Attribute 
    {
        // Type of event that method handles
        public Type Event;

        // Send events if GameObject is enabled or not
        // Default == false, so event will be sent in any case
        public bool OnlyIfEnabled;

        public ExEventHandler(Type type, bool onlyIfEnabled = false){
            this.Event = type;
            this.OnlyIfEnabled = onlyIfEnabled;
        }
    }
}