using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Engine.Scripts.Base;
using Engine.Scripts.Common.Structs;
using Engine.Scripts.Managers;
using Engine.Scripts.Timelines.Playables;
using Engine.Scripts.Timelines.Playables.NotesSpeed;
using ModestTree;
using StringDrop;
using UnityEngine;
using UnityEngine.Timeline;
using Zenject;

namespace Engine.Scripts.Timelines
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "RhythmTimelineAsset", menuName = "Dypsloom/Rhythm Timeline/Rhythm Timeline Asset", order = 1)]
	public class RhythmTimelineAsset : TimelineAsset
	{
		[SerializeField, Uuid.UUID] private string _uuid;
		[StringDropList(typeof(MusicalGenre))]
		[SerializeField] private string _genre;
		[SerializeField] private DifficultyTrack _dificultyTrack;
		[SerializeField] private float _bpm = 120;
		[SerializeField] private float _noteSpeed = 3;
		[SerializeField] private string _authour;
		[SerializeField] private string _fullName;
		[SerializeField][TextArea(3, 10)] private string _description;
		[SerializeField] private Texture2D _cover;
		[SerializeField] private bool _preventMaxScoreOvershoot = false;
		[SerializeField] private bool _preventMinScoreOvershoot = false;
		[SerializeField] private int _conditionStar;
		[SerializeField] private FloatRange _spawnRange = new() { Max = 9, Min = 3 };
		[SerializeField] private bool _isSpecial;
		[SerializeField] private bool _isTutorial;
		private const double _differenceTime = 0.01d;

		[NonSerialized] protected IReadOnlyList<IReadOnlyList<TimelineClip>> _beats;

		private AudioTrack _audioTrack;
		private AudioClip _audioClip;

		public bool IsRecalculated { get; private set; } = false;
		public bool IsAudioPreload { get; private set; } = false;

		public string Uuid => _uuid;
		public string Genre => _genre;
		public DifficultyTrack DificultyTrack => _dificultyTrack;
		public float Bpm => _bpm;
		public float NoteSpeed => _noteSpeed;
		public string FullName => _fullName;
		public string Authour => _authour;
		public Texture2D Cover => _cover;
		public string Description => _description;
		public float Crochet => 60f / _bpm;
		public float HalfCrochet => 30f / _bpm;
		public float QuarterCrochet => 15f / _bpm;
		public int ConditionStar => _conditionStar;
		public int RhythmTrackCount => RhythmClips.Count;
		public bool PreventMaxScoreOvershoot => _preventMaxScoreOvershoot;
		public bool PreventMinScoreOvershoot => _preventMinScoreOvershoot;
		public bool IsSpecial => _isSpecial;
		public bool IsTutorial => _isTutorial;
		public IRhythmDirector RhythmDirector { get; private set; }
		public FloatRange SpawnRange => _spawnRange;
		public float SpeedKoefficient { get; private set; } = 1;

		public float CurrentSpeed => NoteSpeed * SpeedKoefficient;

		public AudioClip AudioClip
		{
			get
			{
				if (_audioClip == null)
				{
					var clips = RhythmAudioTrack.GetClips();
					foreach (var clip in clips)
					{
						if (clip.asset is AudioPlayableAsset asset)
							_audioClip = asset.clip;
					}
				}
				return _audioClip;
			}
		}

		public int RhythmClipCount
		{
			get
			{
				var count = 0;
				var trackCount = RhythmClips.Count;
				for (int i = 0; i < trackCount; i++)
					count += _beats[i].Count;

				return count;
			}
		}

		public AudioTrack RhythmAudioTrack
		{
			get
			{
				if (_audioTrack == null)
				{
					_ = new List<IReadOnlyList<TimelineClip>>();

					var outputTracks = GetOutputTracks();

					foreach (var track in outputTracks)
					{
						if (track is AudioTrack rhythmBeatTrack)
						{
							_audioTrack = rhythmBeatTrack;
							continue;
						}
					}
				}

				return _audioTrack;
			}
		}

		public IReadOnlyList<IReadOnlyList<TimelineClip>> RhythmClips
		{
			get
			{
				if (_beats == null)
				{
					List<IReadOnlyList<TimelineClip>> newBeatLists = new();
					var outputTracks = GetOutputTracks();

					foreach (var track in outputTracks)
					{
						if (track is RhythmTrack rhythmBeatTrack)
						{
							var beatList = GetClipsInTrack(rhythmBeatTrack);
							newBeatLists.Add(beatList);
						}
					}
					_beats = newBeatLists;
				}

				return _beats;
			}
		}

		[Inject]
		public void Bind(IRhythmDirector rhythmDirector)
		{
			RhythmDirector = rhythmDirector;
		}

		public void SetSpeedKoefficient(float value)
		{
			SpeedKoefficient = value;
		}
		public void ResetSpeedKoefficient()
		{
			SpeedKoefficient = 1;
		}

		protected IReadOnlyList<TimelineClip> GetClipsInTrack(RhythmTrack beatTrack)
		{
			var beatList = new List<TimelineClip>();

			var clips = beatTrack.GetClips();
			foreach (var clip in clips)
			{
				if (clip.asset is RhythmClip == false)
					continue;
				beatList.Add(clip);
			}

			beatList.Sort((x, y) => x.start.CompareTo(y.start));

			return beatList;
		}

		public async UniTask GameInitiate()
		{
			if (!IsRecalculated)
			{
				CalcTimeline();
			}
			if (!IsAudioPreload)
			{
				await PreloadTrack();
			}
		}

		public async UniTask PreloadTrack()
		{
			var audioClip = AudioClip;

			if (audioClip.loadState == AudioDataLoadState.Loaded)
				return;

			_ = AudioClip.LoadAudioData();

			await UniTask.WaitUntil(() => audioClip.loadState == AudioDataLoadState.Loaded);
			IsAudioPreload = true;
		}
		public void CalcTimeline()
		{
			List<NoteSpeedClip> _speedClips = new();
			List<TimelineClip> _rhythmClips = new();
			var outputTracks = GetOutputTracks();

			foreach (var item in outputTracks)
			{
				if (item is RhythmTrack rhythmTrack)
				{
					var clips = item.GetClips();
					foreach (var clip in clips)
					{
						if (clip.asset is RhythmClip rhythmClip)
						{

							rhythmClip.RhythmPlayableBehaviour.SpecialPoint = false;
							rhythmClip.RhythmPlayableBehaviour.EndPoint = false;
							_rhythmClips.Add(clip);
						}
					}
				}

				if (item is NoteSpeedTrack noteSpeedTrack)
				{
					var clips = item.GetClips();
					foreach (var clip in clips)
					{
						if (clip.asset is NoteSpeedClip noteSpeedClip)
							_speedClips.Add(noteSpeedClip);
					}
				}
			}

			if (_rhythmClips.IsEmpty())
				return;

			_rhythmClips = _rhythmClips.OrderBy(x => x.start).ToList();
			_speedClips = _speedClips.OrderBy(x => x.InTime).ToList();

			// Считаем специальные точки

			float beforeTime = 0;
			foreach (var speedClip in _speedClips)
			{
				float targetTime = (float) speedClip.InTime + _spawnRange.Max;
				ChageStarPoints(_rhythmClips, targetTime, beforeTime);
				beforeTime = targetTime;
				targetTime = (float) speedClip.OutTime + _spawnRange.Max;
				ChageStarPoints(_rhythmClips, targetTime, beforeTime);
				beforeTime = targetTime;
			}

			var outClips = _rhythmClips.FindAll(x => x.start > beforeTime + _differenceTime);
			_rhythmClips
				.FindAll(x => x.start > beforeTime + _differenceTime)
				.ForEach(x => (x.asset as RhythmClip).RhythmPlayableBehaviour.SpecialPoint = false);

			// Считаем финальные

			var lastInTimeLine = _rhythmClips.Last().start;
			_rhythmClips
				.FindAll(x => Math.Abs(x.start - lastInTimeLine) <= _differenceTime)
				.ForEach(x => (x.asset as RhythmClip).RhythmPlayableBehaviour.EndPoint = true);

			IsRecalculated = true;
		}

		private void ChageStarPoints(List<TimelineClip> rhythmClips, double targetTime, double beforeTime)
		{
			var viewClips = rhythmClips
																		.FindAll(x => x.start > beforeTime
																											&& x.start <= targetTime)
																		.OrderByDescending(x => x.start)
																		.ToList();

			if (viewClips.IsEmpty())
				return;

			var lastTimeBeforeSpeedChange = viewClips.First().start;
			viewClips
				.FindAll(x => Math.Abs(x.start - lastTimeBeforeSpeedChange) <= _differenceTime)
				.ForEach(x => (x.asset as RhythmClip).RhythmPlayableBehaviour.SpecialPoint = true);
		}

#if UNITY_EDITOR
		public bool Check(string fileName)
		{
			UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(_uuid), $"Не заполнен Uuid в {fileName}");
			UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(_authour), $"Не заполнен Author в {fileName}");
			UnityEngine.Assertions.Assert.IsFalse(string.IsNullOrEmpty(_fullName), $"Не заполнен FullName в {fileName}");
			UnityEngine.Assertions.Assert.IsFalse(_cover == null, $"Не заполнен Cover в {fileName}");

			return true;
		}

#endif

	}
}