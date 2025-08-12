using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.Left)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.Right)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.Center)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.AngleLeft)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.AngleRight)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.AspectRation)]
	[SocketAction(FloatingWindowVideoStreamMaterialActionAlias.PositionReset)]
	public class FloatingWindowVideoStreamPosition : IncomingAction
	{
		public ulong MaterialId { get; private set; }
		public string Tag { get; private set; }
		
		public override bool Parse()
		{
			MaterialId = Content.GetULong(IncomingPacketDataKey.MaterialId);
			Tag = Content.GetString(IncomingPacketDataKey.Tag);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.FloatingWindowVideoStreamControl, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}
