using Engine.Scripts.Managers;
using UnityEngine.Playables;

namespace Engine.Scripts.Timelines.Playables.NotesSpeed
{
	public class NoteSpeedBehaviour : PlayableBehaviour
	{
		public NoteSpeedClip NoteSpeed { get; set; }

		public bool InTime(double time)
			=> NoteSpeed.InTime <= time && time <= NoteSpeed.OutTime;

		public RhythmTimelineAsset GetParentAsset() => NoteSpeed.ParentAsset;

		public float GetSpeed() => NoteSpeed.Speed;

	}
}
