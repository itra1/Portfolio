using UnityEngine.Playables;

namespace Engine.Scripts.Timelines.Playables.NotesSpeed
{
	public class NoteSpeedMixerBehaviour : PlayableBehaviour
	{
		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{

			var director = playable.GetGraph().GetResolver() as PlayableDirector;

			if (director == null)
				return;

			var time = director.time;

			int inputCount = playable.GetInputCount();

			if (inputCount == 0)
				return;

			var timelineAsset = director.playableAsset as RhythmTimelineAsset;

			float speedKoefficient = timelineAsset.SpeedKoefficient;

			for (int i = 0; i < inputCount; i++)
			{
				var inputPlayable = (ScriptPlayable<NoteSpeedBehaviour>) playable.GetInput(i);

				var input = inputPlayable.GetBehaviour();

				if (input.InTime(time))
				{
					speedKoefficient = input.GetSpeed();
				}
			}
			if (speedKoefficient != timelineAsset.SpeedKoefficient)
				timelineAsset.SetSpeedKoefficient(speedKoefficient);
		}
	}
}
