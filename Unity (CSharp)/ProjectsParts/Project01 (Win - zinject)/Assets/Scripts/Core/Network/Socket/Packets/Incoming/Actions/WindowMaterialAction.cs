using com.ootii.Messages;
using Core.Messages;
using Core.Network.Socket.Attributes;
using Core.Network.Socket.Packets.Incoming.Actions.Consts;
using Core.Network.Socket.Packets.Incoming.States;

namespace Core.Network.Socket.Packets.Incoming.Actions
{
	[SocketAction(WindowMaterialActionAlias.VideoSetLoop)]
	[SocketAction(WindowMaterialActionAlias.PrevSlide)]
	[SocketAction(WindowMaterialActionAlias.NextSlide)]
	[SocketAction(WindowMaterialActionAlias.ListUp)]
	[SocketAction(WindowMaterialActionAlias.ListDown)]
	[SocketAction(WindowMaterialActionAlias.ZoomIn)]
	[SocketAction(WindowMaterialActionAlias.ZoomOut)]
	[SocketAction(WindowMaterialActionAlias.PageDown)]
	[SocketAction(WindowMaterialActionAlias.PageUp)]
	[SocketAction(WindowMaterialActionAlias.PageLeft)]
	[SocketAction(WindowMaterialActionAlias.PageRight)]
	[SocketAction(WindowMaterialActionAlias.PagePrevious)]
	[SocketAction(WindowMaterialActionAlias.PageNext)]
	[SocketAction(WindowMaterialActionAlias.Play)]
	[SocketAction(WindowMaterialActionAlias.PlayLoop, WindowMaterialActionAlias.VideoSetLoop)]
	[SocketAction(WindowMaterialActionAlias.Pause)]
	[SocketAction(WindowMaterialActionAlias.Reset)]
	[SocketAction(WindowMaterialActionAlias.FullscreenToggle)]
	[SocketAction(WindowMaterialActionAlias.PageBack)]
	[SocketAction(WindowMaterialActionAlias.PageForward)]
	[SocketAction(WindowMaterialActionAlias.Save)]
	[SocketAction(WindowMaterialActionAlias.Reload)]
	[SocketAction(WindowMaterialActionAlias.Focus)]
	[SocketAction(WindowMaterialActionAlias.SlideNext)]
	[SocketAction(WindowMaterialActionAlias.SlidePrevious)]
	[SocketAction(FloatingWindowMaterialActionAlias.PictureZoomIn, WindowMaterialActionAlias.ZoomIn)]
	[SocketAction(FloatingWindowMaterialActionAlias.PictureZoomOut, WindowMaterialActionAlias.ZoomOut)]
	[SocketAction(FloatingWindowMaterialActionAlias.VideoPlay, WindowMaterialActionAlias.Play)]
	[SocketAction(FloatingWindowMaterialActionAlias.VideoSetLoop, WindowMaterialActionAlias.VideoSetLoop)]
	[SocketAction(FloatingWindowMaterialActionAlias.VideoPause, WindowMaterialActionAlias.Pause)]
	[SocketAction(FloatingWindowMaterialActionAlias.VideoReset, WindowMaterialActionAlias.Reset)]
	[SocketAction(FloatingWindowMaterialActionAlias.VideoFullscreenToggle, WindowMaterialActionAlias.FullscreenToggle)]
	[SocketAction(FloatingWindowMaterialActionAlias.DocumentPageUp, WindowMaterialActionAlias.PageUp)]
	[SocketAction(FloatingWindowMaterialActionAlias.DocumentPageDown, WindowMaterialActionAlias.PageDown)]
	[SocketAction(FloatingWindowMaterialActionAlias.DocumentPageLeft, WindowMaterialActionAlias.PageLeft)]
	[SocketAction(FloatingWindowMaterialActionAlias.DocumentPageRight, WindowMaterialActionAlias.PageRight)]
	[SocketAction(FloatingWindowMaterialActionAlias.DocumentFullscreenToggle, WindowMaterialActionAlias.FullscreenToggle)]
	[SocketAction(FloatingWindowMaterialActionAlias.WordPageUp, WindowMaterialActionAlias.PageUp)]
	[SocketAction(FloatingWindowMaterialActionAlias.WordPageDown, WindowMaterialActionAlias.PageDown)]
	[SocketAction(FloatingWindowMaterialActionAlias.WordFullscreenToggle, WindowMaterialActionAlias.FullscreenToggle)]
	[SocketAction(FloatingWindowMaterialActionAlias.PresentationPageUp, WindowMaterialActionAlias.PageUp)]
	[SocketAction(FloatingWindowMaterialActionAlias.PresentationPageDown, WindowMaterialActionAlias.PageDown)]
	[SocketAction(FloatingWindowMaterialActionAlias.BrowserPageBack, WindowMaterialActionAlias.PageBack)]
	[SocketAction(FloatingWindowMaterialActionAlias.BrowserPageForward, WindowMaterialActionAlias.PageForward)]
	[SocketAction(FloatingWindowMaterialActionAlias.BrowserReload, WindowMaterialActionAlias.Reload)]
	public class WindowMaterialAction : IncomingAction
	{
		public ulong MaterialId { get; private set; }
		public ulong? AreaId { get; private set; }
		public ulong? EpisodeId { get; private set; }
		public ulong? StatusContentId { get; private set; }
		public string Uuid { get; private set; }
		
		public bool IsFloatWindow => AreaId == null && EpisodeId == null && StatusContentId == null;
		
		public override bool Parse()
		{
			MaterialId = Content.GetULong("materialId");
			
			if (Content.ContainsKey("areaId"))
				AreaId = Content.GetULong("areaId");
			if (Content.ContainsKey("episodeId"))
				EpisodeId = Content.GetULong("episodeId");
			if (Content.ContainsKey("statusContentId"))
				StatusContentId = Content.GetULong("statusContentId");
			if (Content.ContainsKey("uuid"))
				Uuid = Content.GetString("uuid");
			
			return true;
		}

		public override bool Process()
		{
			if (!base.Process())
				return false;
			
			MessageDispatcher.SendMessage(this, MessageType.WindowMaterialAction, this, EnumMessageDelay.IMMEDIATE);
			return true;
		}
	}
}