using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data
{
    public class SoundVolumeState
    {
        [MaterialDataPropertyParse("mute")]
        public bool Mute { get; set; }
        
        [MaterialDataPropertyParse("level")]
        public int Level { get; set; }
    }
}