using System;
using System.Collections.Generic;
using Core.Materials.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Data;
using Core.UI.Timers.Data;

namespace Core.Network.Socket.Packets.Outgoing.States.Common
{
	/// <summary>
	/// Устаревшее название - "StateMessage"
	/// </summary>
	public partial class OutgoingState
	{
		public DesktopData GetDesktop() => desktop;
		public Presentation GetPresentation() => presentation;
		public Status GetStatus() => status;
		
		public void SetCurrentScreen(string value) => current_screen = value;
		
		public void SetScreenAsLocked(bool value) => lock_screen = value;
		
		public void SetSystemInfo(string version, string uuid, string buildGuid)
		{
			system_info.version = version;
			system_info.uuid = uuid;
			system_info.buildGUID = buildGuid;
		}
		
		public void AddWidget(ulong materialId, ulong parentAreaId)
		{
			for (var i = widgets.Count - 1; i >= 0; i--)
			{
				var widget = widgets[i];
				
				if (widget.MaterialId == materialId && widget.AreaId == parentAreaId) 
					return;
			}
			
			widgets.Add(new WidgetData
			{
				AreaId = parentAreaId,
				MaterialId = materialId,
				IsFocus = false,
				IsExtend = false
			});
		}
		
		public void UpdateWidget(ulong materialId, ulong parentAreaId, bool inFocus, bool isMaximized)
		{
			for (var i = widgets.Count - 1; i >= 0; i--)
			{
				var widget = widgets[i];
				
				if (widget.MaterialId != materialId || widget.AreaId != parentAreaId) 
					continue;
				
				widget.IsFocus = inFocus;
				widget.IsExtend = isMaximized;
				return;
			}
		}

		public void RemoveWidget(ulong materialId, ulong parentAreaId)
		{
			for (var i = widgets.Count - 1; i >= 0; i--)
			{
				var widget = widgets[i];

				if (widget.MaterialId != materialId || widget.AreaId != parentAreaId) 
					continue;
				
				widgets.RemoveAt(i);
				return;
			}
		}
		
		public void SetTimerInfo(TimerType type, bool running, bool paused, bool visible, bool isAlarmOn, long endTime, long currentTime)
		{
			TimerInfo info;
			
			switch (type)
			{
				case TimerType.Hour:
					info = timers.hour;
					break;
				case TimerType.Minute:
					info = timers.minute;
					break;
				default:
					return;
			}
			
			info.active = running;
			info.paused = paused;
			info.display = visible;
			info.alarm = isAlarmOn;
			info.timeEnd = endTime;
			info.residue = currentTime;
		}
		
		public void SetTimerPosition(TimerType type, float x, float y)
		{
			TimerInfo info;
			
			switch (type)
			{
				case TimerType.Hour:
					info = timers.hour;
					break;
				case TimerType.Minute:
					info = timers.minute;
					break;
				default:
					return;
			}
			
			info.x = x;
			info.y = y;
		}
		
		public void SetTimerColor(TimerType type, string color)
		{
			TimerInfo info;
			
			switch (type)
			{
				case TimerType.Hour:
					info = timers.hour;
					break;
				case TimerType.Minute:
					info = timers.minute;
					break;
				default:
					return;
			}
			
			info.color = color;
		}
		
		public void SetStopwatchInfo(bool running, bool paused, bool visible, long startTime, long? pauseTime, long currentTime, List<long> laps)
		{
			stopwatch.active = running;
			stopwatch.paused = paused;
			stopwatch.display = visible;
			stopwatch.time_start = startTime;
			stopwatch.time_paused = pauseTime;
			stopwatch.total_time = currentTime;
			stopwatch.laps = laps;
		}
		
		public void SetStopwatchPosition(float x, float y)
		{
			stopwatch.x = x;
			stopwatch.y = y;
		}
		
		public void SetStopwatchColor(string color) => stopwatch.color = color;
		
		public void SetMusicPlayerState(ulong trackId, bool looping, bool playing, int time, int timerChanged)
		{
			musicPlayer.trackId = trackId;
			musicPlayer.isLoop = looping;
			musicPlayer.isPlay = playing;
			musicPlayer.time = time;
			musicPlayer.timerChanged = timerChanged;
		}
		
		public void SetScreensaverState(bool isVisible, string type, ulong? materialId)
		{
			screensaver = new ScreensaverData
			{
				visibility = isVisible,
				type = type,
				materialId = materialId
			};
		}
		
		public void SetDesktopAsPreloaded(ulong desktopId) => SetAsLoaded(ref desktop_loaded, desktopId);
		public void SetDesktopAsUnloaded(ulong desktopId) => SetAsUnloaded(ref desktop_loaded, desktopId);
		
		public void SetPresentationAsPreloaded(ulong presentationId) => SetAsLoaded(ref presentation_loaded, presentationId);
		public void SetPresentationAsUnloaded(ulong presentationId) => SetAsUnloaded(ref presentation_loaded, presentationId);
		
		public void SetStatusAsPreloaded(ulong statusId) => SetAsLoaded(ref status_loaded, statusId);
		public void SetStatusAsUnloaded(ulong statusId) => SetAsUnloaded(ref status_loaded, statusId);
		
		public IReadOnlyList<ulong> GetLoadedStatuses() => status_loaded;
		
		public void SetStatusAsActiveAt(int column, ulong materialId, ulong areaMaterialId)
		{
			var state = GetStatus();
			
			var activeMaterials = state.active_colum_material;
			var activeAreaMaterials = state.active_materials;
			
			if (column >= activeMaterials.Length)
				Array.Resize(ref activeMaterials, column);
			
			if (column >= activeAreaMaterials.Length)
				Array.Resize(ref activeAreaMaterials, column);

			var columnIndex = column - 1;
			
			activeMaterials[columnIndex] = materialId;
			activeAreaMaterials[columnIndex] = areaMaterialId;
			
			state.active_colum_material = activeMaterials;
			state.active_materials = activeAreaMaterials;
		}
		
		public bool AddVideoState(MaterialData material, ulong? parentAreaId)
		{
			var materialId = material.Id;
			
			for (var i = videoMaterials.Count - 1; i >= 0; i--)
			{
				var videoState = videoMaterials[i];
				
				if (videoState.materialId == materialId && videoState.parentAreaId == parentAreaId)
					return false;
			}
			
			videoMaterials.Add(new VideoState
			{
				materialId = materialId,
				parentAreaId = parentAreaId
			});

			return true;
		}
		
		public bool UpdateVideoState(MaterialData material, ulong? parentAreaId, bool isPlaying, bool isLooping)
		{
			var materialId = material.Id;
			
			for (var i = videoMaterials.Count - 1; i >= 0; i--)
			{
				var videoState = videoMaterials[i];
				
				if (videoState.materialId == materialId && videoState.parentAreaId != parentAreaId)
					continue;
				
				videoState.parentAreaId = parentAreaId;
				videoState.isPlay = isPlaying;
				videoState.isLoop = isLooping;
				
				return true;
			}

			return false;
		}
		
		public bool RemoveVideoState(MaterialData material, ulong? parentAreaId)
		{
			var materialId = material.Id;
			
			for (var i = videoMaterials.Count - 1; i >= 0; i--)
			{
				var videoState = videoMaterials[i];
				
				if (videoState.materialId != materialId || videoState.parentAreaId != parentAreaId) 
					continue;
				
				videoMaterials.RemoveAt(i);
				return true;
			}

			return false;
		}
		
		public void SetWindowState(string uuid, MaterialData material, bool visible, bool fullscreen, bool inFocus, string tagOpen, string sourceOpen)
		{
			if (!current_windows.TryGetValue(uuid, out var currentWindow))
			{
				currentWindow = new WindowData();
				current_windows.Add(uuid, currentWindow);
			}
			
			currentWindow.type = material.Type;
			currentWindow.alias = material.Category;
			currentWindow.material_id = material.Id;
			currentWindow.visibility = visible;
			currentWindow.fullscreen = fullscreen;
			currentWindow.closed = !visible;
			currentWindow.tag_open = tagOpen;
			currentWindow.source_open = sourceOpen;
			currentWindow.in_focus = inFocus;
		}
		
		public void SetFloatingVideoWindowState(string uuid, MaterialData material, bool isPlaying, bool isLooping, long currentTime)
		{
			var materialId = material.Id;
			
			if (current_windows.TryGetValue(uuid, out var currentWindow))
			{
				var video = currentWindow.video;
				
				if (video == null)
				{
					video = new VideoData();
					currentWindow.video = video;
				}
				
				video.playing = isPlaying;
				video.videoTime = currentTime;
				video.unityTime = ((DateTimeOffset) DateTime.UtcNow).ToUnixTimeSeconds();
			}
			
			FloatingWindow floatingWindow = null;
			
			for (var i = floatingWindows.Count - 1; i >= 0; i--)
			{
				var fw = floatingWindows[i];
				
				if (fw.materialId != materialId) 
					continue;
				
				floatingWindow = fw;
				break;
			}
			
			if (floatingWindow == null)
			{
				floatingWindow = new FloatingWindow { materialId = materialId };
				floatingWindows.Add(floatingWindow);
			}
			
			floatingWindow.isPlay = isPlaying ? true : null;
			floatingWindow.isLoop = isLooping ? true : null;
		}
		
		public void SetFloatingVideoStreamWindowState(MaterialData material, string screenPosition, bool isAspectRatioEnabled, bool isResetActionReady)
		{
			var materialId = material.Id;
			
			VideoStreamState videoStreamState = null;
			
			for (var i = windowVideoStream.Count - 1; i >= 0; i--)
			{
				var wvs = windowVideoStream[i];
				
				if (wvs.materialId != materialId)
					continue;
				
				videoStreamState = wvs;
				break;
			}
			
			if (videoStreamState == null)
			{
				videoStreamState = new VideoStreamState { materialId = materialId };
				windowVideoStream.Add(videoStreamState);
			}
			
			videoStreamState.position = screenPosition;
			videoStreamState.aspectRatio = isAspectRatioEnabled;
			videoStreamState.reset = isResetActionReady;
		}
		
		public void RemoveFloatingWindowState(string uuid)
		{
			if (current_windows.ContainsKey(uuid))
				current_windows.Remove(uuid);
		}
		
		public void RemoveFloatingVideoWindowState(MaterialData material)
		{
			var materialId = material.Id;
			
			for (var i = floatingWindows.Count - 1; i >= 0; i--)
			{
				if (floatingWindows[i].materialId != materialId) 
					continue;
				
				floatingWindows.RemoveAt(i);
				break;
			}
		}
		
		public void RemoveFloatingVideoStreamWindowState(MaterialData material)
		{
			var materialId = material.Id;
			
			for (var i = windowVideoStream.Count - 1; i >= 0; i--)
			{
				if (windowVideoStream[i].materialId != materialId)
					continue;
				
				windowVideoStream.RemoveAt(i);
				break;
			}
		}
		
		public void AddStatusPlaylist(int column, ulong statusId)
		{
			foreach (var state in statusPlaylist)
			{
				if (column == state.column && statusId == state.statusId) 
					return;
			}
			
			statusPlaylist.Add(new StatusPlaylistState
			{
				column = column,
				statusId = statusId,
			});
		}

		public void RemoveStatusPlaylist(int column, ulong statusId)
		{
			foreach (var state in statusPlaylist)
			{
				if (column != state.column || statusId != state.statusId) 
					continue;
				
				statusPlaylist.Remove(state);
				break;
			}
		}
		
		public void SetDocumentPageInfo(ulong materialId, ulong parentAreaId, int pageIndex, int totalPagesCount)
		{
			DocumentPageInfo info = null;
			
			for (var i = presentationMaterials.Count - 1; i >= 0; i--)
			{
				var pm = presentationMaterials[i];

				if (pm.materialId != materialId || pm.parentAreaId != parentAreaId) 
					continue;
				
				info = pm;
				break;
			}

			if (info == null)
			{
				info = new DocumentPageInfo
				{
					parentAreaId = parentAreaId,
					materialId = materialId
				};
				
				presentationMaterials.Add(info);
			}
			
			info.slideIndex = pageIndex;
			info.totalSlides = totalPagesCount;
		}
		
		public void RemoveDocumentPageInfo(ulong materialId, ulong parentAreaId)
		{
			for (var i = presentationMaterials.Count - 1; i >= 0; i--)
			{
				var info = presentationMaterials[i];
				
				if (info.materialId != materialId || info.parentAreaId != parentAreaId) 
					continue;
				
				presentationMaterials.RemoveAt(i);
				break;
			}
		}
		
		private void SetAsLoaded(ref ulong[] ids, in ulong id)
		{
			Array.Resize(ref ids, ids.Length + 1);
			ids[^1] = id;
		}
		
		private void SetAsUnloaded(ref ulong[] ids, in ulong id)
		{
			var index = 0;
			var size = ids.Length;
			
			while (index < size)
			{
				if (ids[index] != id)
				{
					index++;
					continue;
				}
				
				size--;
				
				for (var i = index; i < size; i++)
					ids[i] = ids[i + 1];
				
				break;
			}
			
			if (size < ids.Length)
				Array.Resize(ref ids, size);
		}
	}
}