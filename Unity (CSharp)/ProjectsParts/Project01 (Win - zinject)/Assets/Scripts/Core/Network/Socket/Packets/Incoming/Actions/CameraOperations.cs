using Core.Elements.Windows.Camera.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("camera_create")]
	[SocketAction("camera_delete")]
	[SocketAction("camera_update")]
	public class CameraOperations : MaterialOperations<CameraMaterialData>
	{
		
	}
}