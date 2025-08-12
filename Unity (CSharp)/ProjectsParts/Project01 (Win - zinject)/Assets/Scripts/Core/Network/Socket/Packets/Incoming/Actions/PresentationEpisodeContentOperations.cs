using Core.Elements.PresentationEpisode.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("episodecontent_create")]
	[SocketAction("episodecontent_delete")]
	[SocketAction("episodecontent_update")]
	public class PresentationEpisodeContentOperations : MaterialOperations<PresentationEpisodeAreaMaterialData>
	{
		
	}
}