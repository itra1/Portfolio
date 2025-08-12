using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Data;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("episode_set")]
	public class PresentationEpisodeSet : IncomingAction
	{
		private PresentationData _data;
		
		public override bool Parse()
		{
			_data = new PresentationData(Content);
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.PresentationEpisodeSet, _data, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}