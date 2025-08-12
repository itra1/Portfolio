using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("stopwatch_lap_remove")]
    public class StopwatchLapRemove : IncomingAction
    {
        public int Index { get; private set; }
		
        public override bool Parse()
        {
            if (Content.ContainsKey(IncomingPacketDataKey.Index))
                Index = Content.GetInt(IncomingPacketDataKey.Index);
            
            return true;
        }
        
        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            MessageDispatcher.SendMessage(this, MessageType.StopwatchLapRemove, this, EnumMessageDelay.IMMEDIATE);
            return true;
        }
    }
}