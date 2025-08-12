using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("screensaver_visibility")]
	public class ScreensaverVisibility : IncomingAction
	{
		private int? _visibility;

		public override bool Parse()
		{
			_visibility = DataJson.GetInt(IncomingPacketDataKey.ScreensaverVisibility);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.ScreensaverOpen, _visibility, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}