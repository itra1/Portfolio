using Engine.Scripts.Common.Interfaces;
using Engine.Scripts.Managers;
using Game.Shared;
using UnityEngine;
using UnityEngine.Playables;

namespace Engine.Scripts.Scenes
{
	public class SceneBase : MonoBehaviour,
	IPoolParent, ISceneTracks, ISceneNotesParent, ISceneCamera, ISceneUiParents, ISceneAccuracy, ISceneAudioSources, IScenePlayableDirector
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private SpriteRenderer _perfectLine;
		[SerializeField] private Transform _poolParent;
		[SerializeField] private Transform _accuracySpawnPoint;
		[SerializeField] private RectTransform _windowsParent;
		[SerializeField] private RectTransform _popupParent;
		[SerializeField] private RectTransform _splashParent;
		[SerializeField] private Transform _tracksParent;
		[SerializeField] private Transform _noteParent;
		[SerializeField] private TrackObject[] _tracks;
		[SerializeField] private AudioSource[] _audioSources;
		[SerializeField] private PlayableDirector _playableDirector;

		public SpriteRenderer PerfectLine => _perfectLine;
		public Transform PoolParent => _poolParent;
		public Transform AccuracySpawnPoint => _accuracySpawnPoint;
		public RectTransform WindowsParent => _windowsParent;
		public RectTransform PopupParent => _popupParent;
		public RectTransform SplashParent => _splashParent;
		public Camera Camera => _camera;
		public Transform TracksParent => _tracksParent;
		public Transform NotesParent => _noteParent;
		public TrackObject[] Tracks => _tracks;
		public AudioSource[] AudioSources => _audioSources;
		public PlayableDirector PlayableDirector => _playableDirector;
	}
}
