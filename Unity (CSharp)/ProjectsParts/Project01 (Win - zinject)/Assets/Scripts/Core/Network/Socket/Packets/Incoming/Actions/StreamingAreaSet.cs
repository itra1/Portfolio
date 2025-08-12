using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("streaming_area_set")]
	public class StreamingAreaSet : IncomingAction
	{
		private ulong _areaId;
		
		public override bool Parse()
		{
			_areaId = Content.GetULong(IncomingPacketDataKey.AreaId);
			return true;
		}
		
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.StreamingAreaSelect, _areaId, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}