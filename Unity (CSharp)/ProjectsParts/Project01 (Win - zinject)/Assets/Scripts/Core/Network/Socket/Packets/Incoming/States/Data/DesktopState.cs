using Core.Materials.Attributes;

namespace Core.Network.Socket.Packets.Incoming.States.Data
{
    public class DesktopState
    {
        [MaterialDataPropertyParse("desktopId")]
        public ulong? DesktopId { get; set; }
        
        [MaterialDataPropertyParse("activeDesktopId")]
        public ulong? ActiveDesktopId { get; set; }
    }
}