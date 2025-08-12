using Core.Elements.Windows.Cnp.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("kcsystem_create")]
	[SocketAction("kcsystem_delete")]
	[SocketAction("kcsystem_update")]
	public class CnpOperations : MaterialOperations<CnpMaterialData>
	{
		
	}
}