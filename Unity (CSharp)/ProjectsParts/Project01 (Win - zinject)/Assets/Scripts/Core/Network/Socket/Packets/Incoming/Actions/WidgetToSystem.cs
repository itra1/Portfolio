using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("widget_to_system")]
	public class WidgetToSystem : IncomingAction
	{
		public ulong AreaId { get; private set; }
		public ulong WidgetId { get; private set; }
		
		public override bool Parse()
		{
			AreaId = Content.GetULong(IncomingPacketDataKey.AreaId);
			WidgetId = Content.GetULong(IncomingPacketDataKey.WidgetId);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.WidgetToSystem, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}