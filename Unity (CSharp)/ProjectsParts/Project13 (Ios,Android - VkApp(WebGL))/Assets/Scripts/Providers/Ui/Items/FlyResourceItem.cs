using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Providers.Ui.Items {
	public class FlyResourceItem : FlyingItem {
		[SerializeField] protected bool _startAround = true;

		private Vector2 _moveTarget;
		private bool _isMove;
		private float _speed = 5;
		private bool _finalMove;

		public void OnEnable() {
			_finalMove = false;
			_speed = 7;
			_graphic.enabled = true;
			if (_startAround)
				CalcStartPoint();
			else
				MoveFinal();
		}

		public void Update() {
			if (!_isMove)
				return;
			Move();
		}

		private void CalcStartPoint() {
			var direct = (new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f))).normalized;
			_moveTarget = (Vector2)_rectTransform.position + direct * Random.Range(0.1f, 1f);
			_isMove = true;

			_ = UniTask.Create(async () => {
				await UniTask.Delay(UnityEngine.Random.Range(400, 700));
				MoveFinal();
			});
		}

		private void MoveFinal() {
			_moveTarget = _finelMove;
			_speed = 15;
			_finalMove = true;
			_isMove = true;
		}

		private void Move() {
			var newAnchor = (Vector2)_rectTransform.position + ((_moveTarget - (Vector2)_rectTransform.position).normalized * _speed * Time.deltaTime);
			if ((_moveTarget - (Vector2)_rectTransform.position).sqrMagnitude < (newAnchor - (Vector2)_rectTransform.position).sqrMagnitude) {
				_rectTransform.position = _moveTarget;
				_isMove = false;
				if (_finalMove) {
					_graphic.enabled = false;
					_ = UniTask.Create(async () => {
						await UniTask.Delay(500);
						gameObject.SetActive(false);
					});
				}
			} else {
				_rectTransform.position = newAnchor;
			}
		}
	}
}
