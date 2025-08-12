using Core.Network.Socket.Packets.Incoming.Consts;
using Leguar.TotalJSON;

namespace Core.Network.Socket.Packets.Incoming.Actions.Data
{
    public class WindowData
    {
        public bool? Closed { get; }
        public bool? Visibility { get; }
		
        public WindowData(JSON data)
        {
            Closed = data.ContainsKey(IncomingPacketDataKey.Closed) ? data.GetBool(IncomingPacketDataKey.Closed) : null;
            Visibility = data.ContainsKey(IncomingPacketDataKey.Visibility) ? data.GetBool(IncomingPacketDataKey.Visibility) : null;
        }
    }
}