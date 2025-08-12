using Core.Elements.Desktop.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("desktoparea_create")]
	[SocketAction("desktoparea_delete")]
	[SocketAction("desktoparea_update")]
	public class DesktopAreaOperations : MaterialOperations<DesktopAreaMaterialData>
	{
		
	}
}