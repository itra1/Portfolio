using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Data
{
    public interface IMsOfficeRequestData
    {
        IPacket OutgoingPacket { get; set; }
        IPacket IncomingPacket { get; set; }
        
        bool Completed { get; set; }
    }
}