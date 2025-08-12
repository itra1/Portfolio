using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Game.Providers.Ui.Items {
	internal class FlyRoundItem : FlyingItem {
		[SerializeField] private AnimationCurve _changeAnimation;

		private float _moveTime = 0.5f;

		private void OnEnable() {
			_ = Move();
		}

		private async UniTask Move() {

			Vector3 startPosition = (Vector2)transform.position;
			Vector3 starMaxSize = Vector3.one * 3;
			startPosition.z += -1;
			float time = 0;
			Vector3 newAnchor = startPosition;
			float deltaX = _finelMove.x - startPosition.x;
			float deltaY = _finelMove.y - startPosition.y;

			_graphic.transform.localScale = Vector3.zero;
			await _graphic.transform.DOScale(starMaxSize, 0.3f).ToUniTask();

			while (time < 1) {
				await UniTask.Yield();

				time += Time.deltaTime / _moveTime;

				newAnchor.x = startPosition.x + (deltaX * time);
				newAnchor.y = startPosition.y + (deltaY * _changeAnimation.Evaluate(time));

				_graphic.transform.localScale = Vector3.Lerp(starMaxSize, Vector3.one, time);
				_rectTransform.position = newAnchor;
			}
			_graphic.transform.localScale = Vector3.one;
			_rectTransform.position = _finelMove;
			await UniTask.Delay(500);
			gameObject.SetActive(false);
		}
	}
}
