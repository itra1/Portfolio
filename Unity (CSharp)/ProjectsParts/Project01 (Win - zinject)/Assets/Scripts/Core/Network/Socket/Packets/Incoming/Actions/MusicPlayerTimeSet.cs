using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
    [SocketAction("musicPlayer_time_set")]
    public class MusicPlayerTimeSet : IncomingAction
    {
        private int _time;
		
        public override bool Parse()
        {
            _time = Content.GetInt(IncomingPacketDataKey.Time);
            return true;
        }

        public override bool Process()
        {
            if (!base.Process())
                return false;
			
            MessageDispatcher.SendMessage(this, MessageType.MusicPlayerTimeSet, _time, EnumMessageDelay.IMMEDIATE);
            return true;
        }
    }
}