using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("presentation_set")]
	public class PresentationSet : IncomingAction
	{
		private ulong _presentationId;

		public override bool Parse()
		{
			_presentationId = Content.GetULong(IncomingPacketDataKey.Id);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.PresentationSet, _presentationId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}