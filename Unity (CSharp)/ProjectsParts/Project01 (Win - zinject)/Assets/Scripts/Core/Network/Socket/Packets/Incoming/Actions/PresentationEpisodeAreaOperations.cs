using Core.Elements.PresentationEpisode.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("episodearea_create")]
	[SocketAction("episodearea_delete")]
	[SocketAction("episodearea_update")]
	public class PresentationEpisodeAreaOperations : MaterialOperations<PresentationEpisodeAreaMaterialData>
	{
		
	}
}