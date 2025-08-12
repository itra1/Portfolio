using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("window_video_set")]
	public class FloatingWindowMaterialOpen : IncomingAction
	{
		public string Tag => "video";
		public ulong MaterialId { get; private set; }

		public override bool Parse()
		{
			MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.FloatingWindowMaterialOpen, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}
