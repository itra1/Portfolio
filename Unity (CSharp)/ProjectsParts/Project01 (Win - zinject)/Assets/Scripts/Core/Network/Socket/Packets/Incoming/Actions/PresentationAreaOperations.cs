using Core.Elements.Presentation.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("presentationarea_create")]
	[SocketAction("presentationarea_delete")]
	[SocketAction("presentationarea_update")]
	public class PresentationAreaOperations : MaterialOperations<PresentationAreaMaterialData>
	{
		
	}
}