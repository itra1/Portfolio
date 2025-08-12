using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets
{
    [PacketName(PacketName.DocumentStateSet)]
    public class DocumentStateSet : PacketBase
    {
        public int Zoom { get; set; }
        public int Scroll { get; set; }
    }
}