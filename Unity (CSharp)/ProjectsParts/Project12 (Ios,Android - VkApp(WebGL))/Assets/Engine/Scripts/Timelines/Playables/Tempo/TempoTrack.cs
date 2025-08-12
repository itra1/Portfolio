using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Playables.Tempo
{
	/// <summary>
	/// The tempo track has a custom inspector that lets you put markers at defined intervals.
	/// </summary>
	[TrackColor(0.1f, 0.4103774f, 0.7846687f)]
	[DisplayName("Rhythm/Tempo Track")]
	public class TempoTrack : TrackAsset
	{
#if UNITY_EDITOR
		[Tooltip("The on beat editor settings.")]
		[SerializeField] protected TempoMarkerEditorSettings m_OnBeat;
		[Tooltip("The off beat editor settings.")]
		[SerializeField] protected TempoMarkerEditorSettings m_OffBeat;

		public TempoMarkerEditorSettings OnBeat => m_OnBeat;
		public TempoMarkerEditorSettings OffBeat => m_OffBeat;

		public TempoMarkerEditorSettings GetMarkerEditorSettings(TempoMarker tempoMarker)
		{
			if (m_OnBeat != null && tempoMarker.ID == m_OnBeat.ID)
				return m_OnBeat;

			if (m_OffBeat != null && tempoMarker.ID == m_OffBeat.ID)
				return m_OffBeat;

			return default;
		}
#endif
	}
}