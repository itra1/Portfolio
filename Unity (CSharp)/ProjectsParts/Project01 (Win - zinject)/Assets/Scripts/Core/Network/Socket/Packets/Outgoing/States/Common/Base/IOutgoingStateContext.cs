using System.Collections.Generic;
using Core.Materials.Data;
using Core.Network.Socket.Packets.Outgoing.States.Common.Data;
using Core.UI.Timers.Data;

namespace Core.Network.Socket.Packets.Outgoing.States.Common.Base
{
	/// <summary>
	/// Устаревшее название - "StateMessage"
	/// </summary>
	public interface IOutgoingStateContext
	{
		DesktopData GetDesktop();
		Presentation GetPresentation();
		Status GetStatus();
		
		void SetCurrentScreen(string value);
		
		void SetScreenAsLocked(bool value);

		void SetSystemInfo(string version, string uuid, string buildGuid);
		
		void AddWidget(ulong materialId, ulong parentAreaId);
		void UpdateWidget(ulong materialId, ulong parentAreaId, bool inFocus, bool isMaximized);
		void RemoveWidget(ulong materialId, ulong parentAreaId);
		
		void SetTimerInfo(TimerType type, bool running, bool paused, bool visible, bool isAlarmOn, long endTime, long currentTime);
		void SetTimerPosition(TimerType type, float x, float y);
		void SetTimerColor(TimerType type, string color);
		void SetStopwatchInfo(bool running, bool paused, bool visible, long startTime, long? pauseTime, long currentTime, List<long> laps);
		void SetStopwatchPosition(float x, float y);
		void SetStopwatchColor(string color);
		
		void SetMusicPlayerState(ulong trackId, bool looping, bool playing, int time, int timerChanged);

		void SetScreensaverState(bool isVisible, string type, ulong? materialId);

		void SetDesktopAsPreloaded(ulong desktopId);
		void SetDesktopAsUnloaded(ulong desktopId);

		void SetPresentationAsPreloaded(ulong presentationId);
		void SetPresentationAsUnloaded(ulong presentationId);
		
		void SetStatusAsPreloaded(ulong statusId);
		void SetStatusAsUnloaded(ulong statusId);
		
		IReadOnlyList<ulong> GetLoadedStatuses();
		
		void SetStatusAsActiveAt(int column, ulong materialId, ulong areaMaterialId);
		
		bool AddVideoState(MaterialData material, ulong? parentAreaId);
		bool UpdateVideoState(MaterialData material, ulong? parentAreaId, bool isPlaying, bool isLooping);
		bool RemoveVideoState(MaterialData material, ulong? parentAreaId);
		
		void SetWindowState(string uuid, MaterialData material, bool visible, bool fullscreen, bool inFocus, string tagOpen, string sourceOpen);
		void SetFloatingVideoWindowState(string uuid, MaterialData material, bool isPlaying, bool isLooping, long currentTime);
		void SetFloatingVideoStreamWindowState(MaterialData material, string screenPosition, bool isAspectRatioEnabled, bool isResetActionReady);
		
		void RemoveFloatingWindowState(string uuid);
		void RemoveFloatingVideoWindowState(MaterialData material);
		void RemoveFloatingVideoStreamWindowState(MaterialData material);

		void AddStatusPlaylist(int column, ulong statusId);
		void RemoveStatusPlaylist(int column, ulong statusId);
		
		void SetDocumentPageInfo(ulong materialId, ulong parentAreaId, int pageIndex, int totalPagesCount);
		void RemoveDocumentPageInfo(ulong materialId, ulong parentAreaId);
	}
}