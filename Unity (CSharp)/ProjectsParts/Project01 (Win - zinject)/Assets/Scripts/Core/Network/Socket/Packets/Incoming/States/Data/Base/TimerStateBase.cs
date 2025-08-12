using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data.Base
{
    public abstract class TimerStateBase
    {
        [MaterialDataPropertyParse("paused")]
        public bool Paused { get; set; }
        
        [MaterialDataPropertyParse("display")]
        public bool Display { get; set; }
        
        [MaterialDataPropertyParse("active")]
        public bool Active { get; set; } 
        
        [MaterialDataPropertyParse("x")]
        public float X { get; set; }

        [MaterialDataPropertyParse("y")]
        public float Y { get; set; }
    }
}