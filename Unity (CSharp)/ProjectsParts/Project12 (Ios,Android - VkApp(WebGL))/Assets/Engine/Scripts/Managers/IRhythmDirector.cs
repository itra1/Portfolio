using Cysharp.Threading.Tasks;
using Engine.Scripts.Common.Structs;
using Engine.Scripts.Timelines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Engine.Scripts.Managers
{
	public interface IRhythmDirector
	{
		UnityEvent OnSongPlay { get; set; }
		UnityEvent OnSongEnd { get; set; }
		UnityEvent OnTapEnd { get; set; }

		float NoteSpeed { get; }
		float Bpm { get; }
		float Crochet { get; }
		float HalfCrochet { get; }
		float QuarterCrochet { get; }
		double DspSongStartTime { get; }
		TrackObject[] TrackObjects { get; }
		FloatRange SpawnTimeRange { get; }
		PlayableDirector PlayableDirector { get; }
		IRhythmProcessor RhythmProcessor { get; }
		AudioSource ActiveAudioSource { get; }
		bool IsPlaying { get; }
		RhythmTimelineAsset SongTimelineAsset { get; }

		void ClearPlayableAsset();
		void EndSong();
		void Pause();
		UniTask PlaySong(RhythmTimelineAsset songTimeLine);
		void Rollback();
		void SetRhythmTimelineAsset(RhythmTimelineAsset asset);
		void UnPause();
	}
}