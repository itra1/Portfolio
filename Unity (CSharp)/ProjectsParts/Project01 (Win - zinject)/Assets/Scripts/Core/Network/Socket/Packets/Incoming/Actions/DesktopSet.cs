using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("desktop_set")]
	public class DesktopSet : IncomingAction
	{
		private ulong _desktopId;

		public override bool Parse()
		{
			_desktopId = Content.GetULong(IncomingPacketDataKey.Id);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.DesktopSet, _desktopId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}