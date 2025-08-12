using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Anims.UI {
	[RequireComponent(typeof(Image))]
	public class ImageSpriteToggleUnitaskAnim : MonoBehaviour {

		[SerializeField] private bool _resetOnPlay;
		[SerializeField] private int _timeMs = 1000;
		[SerializeField] private Sprite[] _spritesArray;
		[SerializeField] private Sprite _noPlaySprite;

		private Image _imageComponent;
		private bool _isPlaying;
		private int _currentIndex;

		public bool IsPlaying => _isPlaying;

		private void Awake() {
			_imageComponent = GetComponent<Image>();
		}

		public void Play() {

			if (_isPlaying) {
				Debug.LogError("Is play already");
				return;
			}

			if (_spritesArray.Length == 0) {
				Debug.LogError("No fill array");
				return;
			}

			if (_resetOnPlay)
				_currentIndex = 0;

			_isPlaying = true;
			_ = Animation();
		}

		public void Stop() {
			_isPlaying = false;
			if (_noPlaySprite is not null)
				_imageComponent.sprite = _noPlaySprite;
		}

		public async UniTask Animation() {

			while (_isPlaying) {
				_imageComponent.sprite = _spritesArray[_currentIndex];
				_currentIndex = _currentIndex >= _spritesArray.Length - 1 ? 0 : ++_currentIndex;
				await UniTask.Delay(_timeMs);
			}
		}
	}
}
