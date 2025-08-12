using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("stopwatch_lap")]
	public class StopwatchLap : IncomingAction
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(MessageType.StopwatchLap, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}