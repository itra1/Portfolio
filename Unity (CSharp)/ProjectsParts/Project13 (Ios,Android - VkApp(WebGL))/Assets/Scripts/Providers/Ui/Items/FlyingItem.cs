using UnityEngine;
using UnityEngine.UI;

namespace Game.Providers.Ui.Items {
	public class FlyingItem : MonoBehaviour {
		[SerializeField] protected Image _graphic;
		[SerializeField] protected TrailRenderer _trailRenderer;

		protected Vector2 _finelMove;
		protected RectTransform _rectTransform;

		public void Awake() {
			_rectTransform = transform as RectTransform;
		}

		public void SetTargetPosition(Vector2 finalAnchor) {
			_finelMove = finalAnchor;
		}
	}
}