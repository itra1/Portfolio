using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;
using Debug = Core.Logging.Debug;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("musicPlayer_loop_toggle")]
	public class MusicPlayerLoopToggle : IncomingAction
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (Content != null)
			{
				Debug.LogError($"Content is missing when trying to process incoming packet {GetType().Name}");
				return false;
			}
			
			MessageDispatcher.SendMessage(this, MessageType.MusicPlayerLoopToggle, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}