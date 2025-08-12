using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("status_set")]
	public class StatusSet : IncomingAction
	{
		private ulong _statusId;

		public override bool Parse()
		{
			_statusId = Content.GetULong(IncomingPacketDataKey.Id);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.StatusSet, _statusId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}