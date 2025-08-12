using System;
using System.Collections.Generic;
using BestHTTP.JSON.LitJson;
using Core.Network.Socket.Packets.Outgoing.States.Common.Base;
using Core.Network.Socket.Packets.Outgoing.States.Common.Data;

namespace Core.Network.Socket.Packets.Outgoing.States.Common
{
	/// <summary>
	/// Устаревшее название - "StateMessage"
	/// </summary>
	public partial class OutgoingState : OutgoingPacket, IOutgoingState
	{
		public string current_screen;
		
		public bool lock_screen;
		
		public string selected_preset_id;
		
		public bool current_map_visibility;
		
		public bool taskbar_visibility = true;
		public bool sidebar_visibility = true;
		public bool subsystems_visibility = true;
		
		public DesktopData desktop = new ();
		public Presentation presentation = new ();
		public Status status = new ();
		
		public ulong[] presentation_loaded = Array.Empty<ulong>();
		public ulong[] status_loaded = Array.Empty<ulong>();
		public ulong[] desktop_loaded = Array.Empty<ulong>();
		
		public Dictionary<string, WindowData> current_windows = new ();
		
		public ScreensaverData screensaver;
		
		public SystemInfo system_info = new ();
		
		public Dictionary<string, string> subsistems = new ();
		
		public KeyValuePair<string, string> incident;
		
		public Presets preset = new ();
		
		public MapData current_map = new ();
		
		public Timers timers = new ();
		public StopwatchInfo stopwatch = new ();
		public List<WidgetData> widgets = new ();
		
		public bool material_actions_show;
		
		public MusicPlayerState musicPlayer = new ();
		
		public List<FloatingWindow> floatingWindows = new();
		public List<VideoState> videoMaterials = new ();
		public List<VideoStreamState> windowVideoStream = new();
		
		public List<StatusPlaylistState> statusPlaylist = new();
		
		public List<DocumentPageInfo> presentationMaterials = new();
		
		public override string ToString() => $"{{type: {GetType().Name}, data: {JsonMapper.ToJson(this)}}}";
	}
}