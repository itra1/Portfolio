using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Serializers
{
    public interface IPacketSerializer
    {
        string Serialize(IPacket packet);
    }
}