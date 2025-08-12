using com.ootii.Messages;
using Core.Elements.Status.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("status_create")]
	[SocketAction("status_delete")]
	[SocketAction("status_update")]
	public class StatusOperations : MaterialOperations<StatusMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (IsRemoving)
				MessageDispatcher.SendMessage(this, MessageType.StatusUnload, Material.Id, EnumMessageDelay.IMMEDIATE);
			else if (IsUpdating)
				MessageDispatcher.SendMessage(this, MessageType.StatusUpdate, Material.Id, EnumMessageDelay.IMMEDIATE);
			
			return true;
		}
	}
}