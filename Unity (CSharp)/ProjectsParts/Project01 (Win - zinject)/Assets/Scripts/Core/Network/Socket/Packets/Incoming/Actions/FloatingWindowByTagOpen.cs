using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("tag_material_open")]
	public class FloatingWindowByTagOpen : IncomingAction
	{
		public string Tag { get; private set; }
		public ulong MaterialId { get; private set; }
		
		public override bool Parse()
		{
			Tag = Content.GetString(IncomingPacketDataKey.Tag);
			MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.FloatingWindowOpenByTag, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}