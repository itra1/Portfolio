using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Elements {
	public class RoundIndicator : MonoBehaviour, IPoolable<IMemoryPool> {
		[SerializeField] private GameObject _activeImage;

		private void OnEnable() {
			SetActive(false);
		}

		public void SetActive(bool active) {
			_activeImage.SetActive(active);
		}

		public void OnDespawned() {
		}

		public void OnSpawned(IMemoryPool p1) {
		}
		public class Factory : PlaceholderFactory<RoundIndicator> { }
	}
}
