using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Engine.Scripts.Timelines.Playables.NotesSpeed
{
	[TrackColor(0.1f, 0.9f, 0.7846687f)]
	[DisplayName("Rhythm/Note Speed")]
	[TrackClipType(typeof(NoteSpeedClip))]
	public class NoteSpeedTrack : TrackAsset
	{
		public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
		{
			var playable = ScriptPlayable<NoteSpeedMixerBehaviour>.Create(graph, inputCount);

			return playable;
		}
	}
}
