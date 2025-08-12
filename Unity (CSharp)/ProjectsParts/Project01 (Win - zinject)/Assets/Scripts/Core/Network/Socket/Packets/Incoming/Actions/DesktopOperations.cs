using com.ootii.Messages;
using Core.Elements.Desktop.Data;
using Core.Messages;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("desktop_create")]
	[SocketAction("desktop_delete")]
	[SocketAction("desktop_update")]
	public class DesktopOperations : MaterialOperations<DesktopMaterialData>
	{
		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			if (IsRemoving)
				MessageDispatcher.SendMessage(this, MessageType.DesktopUnload, Material.Id, EnumMessageDelay.IMMEDIATE);
			
			return true;
		}
	}
}