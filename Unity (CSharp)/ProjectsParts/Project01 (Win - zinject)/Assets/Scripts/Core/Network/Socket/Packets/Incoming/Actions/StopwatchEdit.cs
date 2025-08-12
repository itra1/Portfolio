using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("stopwatch_edit")]
	public class StopwatchEdit : IncomingAction
	{
		public bool Display { get; private set; }
		
		public override bool Parse()
		{
			if (Content.ContainsKey(IncomingPacketDataKey.Display))
				Display = Content.GetBool(IncomingPacketDataKey.Display);
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.StopwatchEdit, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}