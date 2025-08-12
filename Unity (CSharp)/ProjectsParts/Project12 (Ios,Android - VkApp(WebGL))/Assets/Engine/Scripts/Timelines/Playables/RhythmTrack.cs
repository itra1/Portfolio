using System.ComponentModel;
using Engine.Scripts.Managers;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Playables
{
	[TrackColor(0.7846687f, 0.4103774f, 1f)]
	[TrackClipType(typeof(RhythmClip))]
	[DisplayName("Rhythm/Rhythm Track")]
	public class RhythmTrack : TrackAsset
	{
		[Tooltip("The Rhythm Track ID.")]
		[SerializeField] protected int m_ID;

		public int ID => m_ID;

		protected IRhythmDirector _rhythmDirector;

		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			var playable = ScriptPlayable<RhythmMixerBehaviour>.Create(graph, inputCount);

			//store the lane starting position in the clips to allow correct calculation of the paths
			//store start times to allow bullet/ship calculations past the clip's length
			if (_rhythmDirector == null)
			{
				_rhythmDirector = (parent as RhythmTimelineAsset).RhythmDirector;

				if (_rhythmDirector == null)
				{
					return playable;
				}
			}

			foreach (var clip in m_Clips)
			{
				var rhythmClip = clip.asset as RhythmClip;
				if (_rhythmDirector.TrackObjects == null
				|| _rhythmDirector.TrackObjects.Length == 0
				|| m_ID >= _rhythmDirector.TrackObjects.Length)
				{
					continue;
				}

				var clipData = new RhythmClipData(rhythmClip,
						_rhythmDirector,
						m_ID,
						clip.start,
						clip.duration);

				rhythmClip.RhythmClipData = clipData;

				SetClipDuration(rhythmClip, clip);
			}

			return playable;
		}

		protected virtual void SetClipDuration(RhythmClip rhythmClip, TimelineClip clip)
		{
			if (rhythmClip?.RhythmPlayableBehaviour?.NoteDefinition == null)
				return;
			rhythmClip.RhythmPlayableBehaviour.NoteDefinition.SetClipDuration(rhythmClip, clip);
		}
	}
}