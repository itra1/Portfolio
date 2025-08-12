namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base
{
    public abstract class PacketBase : IPacket
    {
        public string AppUuid { get; set; }
    }
}