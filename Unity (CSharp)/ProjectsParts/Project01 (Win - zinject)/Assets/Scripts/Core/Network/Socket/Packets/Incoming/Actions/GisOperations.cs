using Core.Elements.Windows.Gis.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("gis_create")]
	[SocketAction("gis_delete")]
	[SocketAction("gis_update")]
	public class GisOperations : MaterialOperations<GisMaterialData>
	{
		
	}
}