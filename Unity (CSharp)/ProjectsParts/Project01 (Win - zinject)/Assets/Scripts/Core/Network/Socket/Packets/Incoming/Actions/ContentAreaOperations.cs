using com.ootii.Messages;
using Core.Materials.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("contentarea_create")]
	[SocketAction("contentarea_delete")]
	[SocketAction("contentarea_update")]
	public class ContentAreaOperations : MaterialOperations<ContentAreaMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			string messageType;
			
			if (IsCreating)
				messageType = MessageType.ContentAreaCreate;
			else if (IsRemoving)
				messageType = MessageType.ContentAreaRemove;
			else
				messageType = MessageType.ContentAreaUpdate;
			
			MessageDispatcher.SendMessage(this, messageType, Material, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}