using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Attributes;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Consts;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets
{
    [PacketName(PacketName.WorkbookStateValue)]
    public class WorkbookStateValue : PacketBase
    {
        public int PagePositionX { get; set; }
        public int PagePositionY { get; set; }
        public int PageIndex { get; set; }
        public int Zoom { get; set; }
    }
}