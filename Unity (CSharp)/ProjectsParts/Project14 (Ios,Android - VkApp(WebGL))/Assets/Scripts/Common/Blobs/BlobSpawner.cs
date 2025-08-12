using Scripts.Common.Blobs;
using Scripts.GameItems.Platforms;
using UnityEngine;
using Zenject;

namespace Scripts.Common {
	/// <summary>
	/// Генератор клякс
	/// </summary>
	public class BlobSpawner {
		private SignalBus _signalBus;
		private Transform _platformParent;
		private Blob.Factory _blobFactory;

		public BlobSpawner(SignalBus signalBus, SceneComponents sceneComponents, Blob.Factory blobFactory) {
			_signalBus = signalBus;
			_platformParent = sceneComponents.PlatformParent;
			_blobFactory = blobFactory;
		}

		public void Spawn(Color color, Vector3 point, Platform platform) {

			if (platform == null)
				return;

			var instance = _blobFactory.Create(color, platform);

			var targetPosition = point;
			targetPosition.y = platform.transform.position.y + 0.005f;

			instance.transform.position = targetPosition;
			instance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
			instance.gameObject.SetActive(true);
			instance.transform.SetParent(platform.transform);
		}
	}
}
