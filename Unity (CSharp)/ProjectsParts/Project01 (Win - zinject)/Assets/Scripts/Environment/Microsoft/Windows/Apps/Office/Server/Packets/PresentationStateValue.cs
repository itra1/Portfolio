using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets
{
    [PacketName(PacketName.PresentationStateValue)]
    public class PresentationStateValue : PacketBase
    {
        public int Slide { get; set; } = 1;
    }
}