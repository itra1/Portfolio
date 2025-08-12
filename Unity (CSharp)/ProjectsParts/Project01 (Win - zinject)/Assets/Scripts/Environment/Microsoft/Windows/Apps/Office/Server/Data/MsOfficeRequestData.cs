using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Data
{
    public class MsOfficeRequestData : IMsOfficeRequestData
    {
        public IPacket OutgoingPacket { get; set; }
        public IPacket IncomingPacket { get; set; }
        
        public bool Completed { get; set; }
    }
}