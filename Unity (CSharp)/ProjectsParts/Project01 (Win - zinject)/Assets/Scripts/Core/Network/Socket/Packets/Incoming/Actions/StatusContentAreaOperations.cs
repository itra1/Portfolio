using Core.Elements.StatusColumn.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("statuscontentarea_create")]
	[SocketAction("statuscontentarea_delete")]
	[SocketAction("statuscontentarea_update")]
	public class StatusContentAreaOperations : MaterialOperations<StatusContentAreaMaterialData>
	{
		
	}
}