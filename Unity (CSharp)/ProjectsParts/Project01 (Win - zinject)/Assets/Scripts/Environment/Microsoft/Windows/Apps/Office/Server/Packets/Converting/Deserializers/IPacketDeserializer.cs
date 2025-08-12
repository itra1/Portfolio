using System;
using Core.Base;
using Environment.Microsoft.Windows.Apps.Office.Server.Packets.Base;

namespace Environment.Microsoft.Windows.Apps.Office.Server.Packets.Converting.Deserializers
{
    public interface IPacketDeserializer : ILateInitialized, IDisposable
    {
        IPacket Deserialize(string rawData);
    }
}