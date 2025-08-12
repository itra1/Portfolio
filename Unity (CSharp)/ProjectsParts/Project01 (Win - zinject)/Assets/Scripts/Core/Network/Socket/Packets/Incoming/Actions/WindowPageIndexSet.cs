using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("window_page_index_set")]
    public class WindowPageIndexSet : IncomingAction
    {
        public ulong AreaId { get; private set; }
        public ulong MaterialId { get; private set; }
        public int PageIndex { get; private set; }
        
        public override bool Parse()
        {
            AreaId = Content.GetULong(IncomingPacketDataKey.AreaId);
            MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
            PageIndex = Content.GetInt(IncomingPacketDataKey.PageIndex);
            return true;
        }
        
        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            MessageDispatcher.SendMessage(this, MessageType.WindowPageIndexSet, this, EnumMessageDelay.IMMEDIATE);
            return true;
        }
    }
}