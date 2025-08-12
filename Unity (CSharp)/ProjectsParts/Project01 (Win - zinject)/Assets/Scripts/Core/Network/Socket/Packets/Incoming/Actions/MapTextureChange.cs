using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("map_texture")]
	public class MapTextureChange : IncomingAction
	{
		private string _textureId;

		public override bool Parse()
		{
			_textureId = DataJson.GetString(IncomingPacketDataKey.CurrentTextureId);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.MapSetLayer, _textureId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}