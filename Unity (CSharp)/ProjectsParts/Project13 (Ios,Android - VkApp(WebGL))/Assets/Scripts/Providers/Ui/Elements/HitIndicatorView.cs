using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Providers.Ui.Elements {
	public class HitIndicatorView :MonoBehaviour, IPoolable<IMemoryPool> {
		[SerializeField] private Image _image;
		[SerializeField] private Color _activeColor;
		[SerializeField] private Color _deactiveColor;

		private IMemoryPool _pool;

		public void IsActive(bool isActive) {
			_image.color = isActive ? _activeColor : _deactiveColor;
		}
		public void OnDespawned() {
		}
		public void Despawn() {
			_pool.Despawn(this);
		}

		public void OnSpawned(IMemoryPool pool) {
			_pool = pool;
		}
		public class Factory :PlaceholderFactory<HitIndicatorView> { }
	}
}
