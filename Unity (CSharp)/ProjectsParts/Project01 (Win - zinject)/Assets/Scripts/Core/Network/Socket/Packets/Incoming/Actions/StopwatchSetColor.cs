using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("set_stopwatch_color")]
    public class StopwatchSetColor : IncomingAction
    {
        public string Color { get; private set; }

        public override bool Parse()
        {
            if (Content.ContainsKey(IncomingPacketDataKey.Color))
                Color = Content.GetString(IncomingPacketDataKey.Color);
            
            return true;
        }
        
        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            MessageDispatcher.SendMessage(this, MessageType.StopwatchSetColor, this, EnumMessageDelay.IMMEDIATE);
            return true;
        }
    }
}