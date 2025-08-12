using UnityEngine;

namespace Scripts.Common.Blobs {
	public class BlobSettings {

		[SerializeField] private GameObject _prefabSettings;

		public GameObject PrefabSettings => _prefabSettings;
	}
}
