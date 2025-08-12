using System;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Common.Structs;
using Engine.Scripts.Settings;
using Engine.Scripts.Timelines;
using Engine.Scripts.Timelines.Notes.Base;
using Engine.Scripts.Timelines.Notes.Factorys;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Engine.Scripts.Managers
{
	public class RhythmDirector : IRhythmDirector
	{
		[HideInInspector] public UnityEvent OnSongPlay { get; set; } = new();
		[HideInInspector] public UnityEvent OnSongEnd { get; set; } = new();
		[HideInInspector] public UnityEvent OnTapEnd { get; set; } = new();

		private readonly IDspTime _dspTime;
		private readonly ISceneTracks _sceneTrack;
		private readonly ISceneAudioSources _sceneAudioSources;
		private readonly RhythmSettings _rhythmSettings;
		private readonly IScenePlayableDirector _scenePlayableDirector;
		private readonly INoteFactory _noteFactory;
		private RhythmTimelineAsset _songTimelineAsset;
		private bool _isPlaying;
		private double _pausedTime;
		private double _dspSongStartTime;
		private int _audioTracksCount = 0;

		public bool IsPlaying => _isPlaying;
		public IRhythmProcessor RhythmProcessor { get; private set; }
		public PlayableDirector PlayableDirector => _scenePlayableDirector.PlayableDirector;
		public float NoteSpeed => SongTimelineAsset.CurrentSpeed;
		public float Bpm => SongTimelineAsset.Bpm;
		public float Crochet => 60f / Bpm;
		public float HalfCrochet => 30f / Bpm;
		public float QuarterCrochet => 15f / Bpm;
		public TrackObject[] TrackObjects => _sceneTrack.Tracks;
		public FloatRange SpawnTimeRange => SongTimelineAsset.SpawnRange;
		public double DspSongStartTime => _dspSongStartTime;
		public AudioSource ActiveAudioSource => _sceneAudioSources.AudioSources[0];
		public RhythmTimelineAsset SongTimelineAsset
		{
			get
			{
				if (!Application.isPlaying)
					return PlayableDirector.playableAsset as RhythmTimelineAsset;

				return _songTimelineAsset != null
						? _songTimelineAsset
						: PlayableDirector.playableAsset as RhythmTimelineAsset;
			}
		}

		public RhythmDirector(
			IDspTime dspTime,
			ISceneTracks sceneTrack,
			ISceneAudioSources sceneAudioSources,
			IRhythmProcessor rhythmProcessor,
			RhythmSettings rhythmSettings,
			IScenePlayableDirector scenePlayableDirector
		)
		{
			_dspTime = dspTime;
			_sceneTrack = sceneTrack;
			_sceneAudioSources = sceneAudioSources;
			_rhythmSettings = rhythmSettings;
			_scenePlayableDirector = scenePlayableDirector;
			RhythmProcessor = rhythmProcessor;
			RhythmProcessor.SetRhythmDirector(this);

			PlayableDirector.timeUpdateMode = DirectorUpdateMode.DSPClock;
			PlayableDirector.stopped += HandleSongEnded;
		}

		public void SetRhythmTimelineAsset(RhythmTimelineAsset asset)
		{
			PlayableDirector.playableAsset = asset;
		}

		public void Rollback()
		{
			PlayableDirector.time = Math.Max(0, PlayableDirector.time - 2);
			var notes = GameObject.FindObjectsByType<Note>(FindObjectsSortMode.None);
			foreach (var item in notes)
			{
				item.SetNormalEventType();
			}
		}

		public async UniTask PlaySong(RhythmTimelineAsset songTimeLine)
		{
			bool isNewTrack = _songTimelineAsset == null || _songTimelineAsset != songTimeLine;
			_songTimelineAsset = songTimeLine;
			PlayableDirector.playableAsset = _songTimelineAsset;

			ClearActiveNotes();
			SetupTrackBindings();

			if (isNewTrack)
			{
				await (PlayableDirector.playableAsset as RhythmTimelineAsset).GameInitiate();
			}
			PlayableDirector.RebuildGraph();
			PlayableDirector.time = 0.0;
			PlayableDirector.Play();
			_dspSongStartTime = _dspTime.AdaptiveTime;
			_isPlaying = true;

			OnSongPlay?.Invoke();
		}

		protected virtual void SetupTrackBindings()
		{
			var outputTracks = _songTimelineAsset.GetOutputTracks();
			foreach (var track in outputTracks)
			{
				if (track is AudioTrack audioTrack)
					SetUpAudioTrackBinding(audioTrack);
			}
		}

		protected void SetUpAudioTrackBinding(AudioTrack audioTrack)
		{
			PlayableDirector.SetGenericBinding(audioTrack, _sceneAudioSources.AudioSources[_audioTracksCount]);
			_audioTracksCount++;
		}

		protected void HandleSongEnded(PlayableDirector playableDirector)
		{
			OnSongEnd?.Invoke();
		}

		public void EndSong()
		{
			if (!_isPlaying)
				return;
			_isPlaying = false;

			ClearActiveNotes();

			_audioTracksCount = 0;

			//var outputTracks = _songTimelineAsset.GetOutputTracks();
			//foreach (var track in outputTracks)
			//{
			//	if (track.GetType() == typeof(AudioTrack))
			//	{
			//		var audioClips = track.GetClips();
			//		foreach (var audioClip in audioClips)
			//		{
			//			audioClip.start -= AudioDelay;
			//		}
			//	}
			//}

			OnSongEnd?.Invoke();
		}

		public void ClearPlayableAsset()
		{
			PlayableDirector.playableAsset = null;
		}

		public virtual void ClearActiveNotes()
		{
			if (TrackObjects == null)
				return;

			for (int i = 0; i < TrackObjects.Length; i++)
				TrackObjects[i].ClearNotes();
		}

		public void Pause()
		{
			if (PlayableDirector == null)
				return;

			_pausedTime = PlayableDirector.time;
			var count = PlayableDirector.playableGraph.GetRootPlayableCount();
			for (int i = 0; i < count; i++)
			{
				PlayableDirector.playableGraph.GetRootPlayable(i).SetSpeed(0);
			}
		}

		public void UnPause()
		{
			if (PlayableDirector == null)
				return;

			PlayableDirector.time = _pausedTime;
			var count = PlayableDirector.playableGraph.GetRootPlayableCount();
			for (int i = 0; i < count; i++)
			{
				PlayableDirector.playableGraph.GetRootPlayable(i).SetSpeed(1);
			}
		}
	}
}
