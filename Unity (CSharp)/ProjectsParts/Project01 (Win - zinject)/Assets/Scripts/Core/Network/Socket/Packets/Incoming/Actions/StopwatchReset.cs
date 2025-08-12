using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;
using Core.UI.Timers.Data;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("stopwatch_reset")]
	public class StopwatchReset : IncomingAction
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.StopwatchReset, TimerType.Stopwatch, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}