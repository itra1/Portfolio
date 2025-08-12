using com.ootii.Messages;
using Core.Elements.PresentationEpisode.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("episode_create")]
	[SocketAction("episode_delete")]
	[SocketAction("episode_update")]
	public class PresentationEpisodeOperations : MaterialOperations<PresentationEpisodeMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			string messageType;
			
			if (IsCreating)
				messageType = MessageType.PresentationEpisodePreload;
			else if (IsRemoving)
				messageType = MessageType.PresentationEpisodeUnload;
			else
				messageType = MessageType.PresentationEpisodeUpdate;
			
			MessageDispatcher.SendMessage(this, messageType, Material, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}