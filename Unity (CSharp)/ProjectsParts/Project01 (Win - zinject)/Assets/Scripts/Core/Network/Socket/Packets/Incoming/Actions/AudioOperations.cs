using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("audio_create")]
	[SocketAction("audio_delete")]
	[SocketAction("audio_update")]
	public class AudioOperations : MaterialOperations<AudioMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			string messageType;
			
			if (IsCreating)
				messageType = MessageType.AudioPreload;
			else if (IsRemoving)
				messageType = MessageType.AudioUnload;
			else
				messageType = MessageType.AudioUpdate;
			
			MessageDispatcher.SendMessageData(messageType, Material);
			return true;
		}
	}
}