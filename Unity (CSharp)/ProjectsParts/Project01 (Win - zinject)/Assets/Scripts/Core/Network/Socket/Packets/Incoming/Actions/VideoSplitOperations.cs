using Core.Elements.Windows.VideoSplit.Data;
using Core.Network.Socket.Attributes;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction("video_create")]
	[SocketAction("video_delete")]
	[SocketAction("video_update")]
	public class VideoSplitOperations : MaterialOperations<VideoSplitMaterialData>
	{
		
	}
}