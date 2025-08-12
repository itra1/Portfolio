using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;
using Core.UI.SplashScreens.Screensaver.Data.Consts;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("screensaver_set")]
	public class ScreensaverSet : IncomingAction
	{
		public bool IsVisible { get; private set; }
		public string Type { get; private set; }
		public ulong? MaterialId { get; private set; }
		
		public override bool Parse()
		{
			IsVisible = Content.GetBool(IncomingPacketDataKey.Visibility);
			Type = Content.GetString(IncomingPacketDataKey.Type);
			
			if (!Content.IsJNull(IncomingPacketDataKey.MaterialId))
			{
				MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
				Type = ScreensaverType.Material;
			}
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.ScreensaverSet, this, EnumMessageDelay.NEXT_UPDATE);
			return true;
		}
	}
}