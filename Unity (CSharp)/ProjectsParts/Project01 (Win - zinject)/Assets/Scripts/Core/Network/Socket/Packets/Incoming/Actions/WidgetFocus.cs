using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("widget_focus")]
	public class WidgetFocus : IncomingAction
	{
		private ulong _widgetId;

		public override bool Parse()
		{
			_widgetId = Content.GetULong(IncomingPacketDataKey.WidgetId);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.WidgetFocus, _widgetId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}