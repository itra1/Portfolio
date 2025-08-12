using Core.Engine.uGUI;
using SoundPoint;
using UnityEngine;
using Utils;
using Zenject;

namespace Game.Players.Skins {
	public class SkinSound : MonoBehaviour, IZInjection {

		[SerializeField] private AudioClip[] _jumpClips;
		[SerializeField] private FloatRange _jumpClipPitchRange;
		[SerializeField, Range(0, 1)] private float _jumpCliprobability = 0.5f;

		[SerializeField] private AudioClip[] _floorNextClips;
		[SerializeField] private FloatRange _floorNextClipPitchRange;

		[SerializeField] private AudioClip[] _floorFlyClips;
		[SerializeField] private AudioClip[] _fireClips;
		[SerializeField] private AudioClip[] _damageClips;
		[SerializeField] private AudioClip[] _winClips;

		private IAudioPointFactory _audioClipFactory;
		private IAudioPoint _jumpPoint;

		[Inject]
		public void Constructor(IAudioPointFactory audioClipFactory) {
			_audioClipFactory = audioClipFactory;

			_jumpPoint ??= _audioClipFactory.Create();
		}

		/// <summary>
		/// Прыдок
		/// </summary>
		public void Jump() {
			if (_jumpClips.Length == 0)
				return;

			if (_jumpCliprobability >= Random.value) {
				_ = _jumpPoint.SetPitch(_jumpClipPitchRange.Random())
				.PlayRandom(_jumpClips);
			}
		}

		/// <summary>
		/// Перепрыгивание на следующую платформу
		/// </summary>
		public void JumpNext() {
			if (_floorNextClips.Length == 0)
				return;
			_ = _jumpPoint.SetPitch(_floorNextClipPitchRange.Random())
			.PlayRandom(_floorNextClips);
		}

		/// <summary>
		/// Победный звук
		/// </summary>
		public void Win() {
			if (_winClips.Length == 0)
				return;

			_ = _audioClipFactory.Create()
			.AutoDespawn()
			.PlayRandom(_winClips);
		}

		/// <summary>
		/// Победный звук
		/// </summary>
		public void Damage() {
			if (_damageClips.Length == 0)
				return;

			_ = _audioClipFactory.Create()
			.AutoDespawn()
			.PlayRandom(_damageClips);
		}
	}
}
