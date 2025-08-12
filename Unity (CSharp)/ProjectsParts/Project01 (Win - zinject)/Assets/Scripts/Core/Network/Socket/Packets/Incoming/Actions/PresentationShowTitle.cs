using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("presentation_show_names")]
	public class PresentationShowTitle : IncomingAction
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(MessageType.PresentationShowTitle);
			return true;
		}
	}
}