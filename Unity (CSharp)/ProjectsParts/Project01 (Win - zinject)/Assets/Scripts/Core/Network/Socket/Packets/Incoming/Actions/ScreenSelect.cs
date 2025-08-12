using com.ootii.Messages;
using Core.Elements.ScreenModes;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("screen_select")]
	public class ScreenSelect : IncomingAction
	{
		private string _screenMode;
		
		public override bool Parse()
		{
			_screenMode = DataJson.GetString(IncomingPacketDataKey.CurrentScreen);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessageData(MessageType.ScreenSelect, ScreenModeConverter.Convert(_screenMode));
			return true;
		}
	}
}