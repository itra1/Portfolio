using Scripts.Common.Blobs;
using UnityEngine;
using Zenject;

namespace Game.Settings {
	[CreateAssetMenu(fileName = "GamePrefabSettings", menuName = "App/Create/Settings/GamePrefabSettings", order = 2)]
	public class GamePrefabSettings : ScriptableObjectInstaller {

		[SerializeField] private Prefabs _prefabs;

		public override void InstallBindings() {
			_ = Container.BindInterfacesAndSelfTo<Prefabs>().FromInstance(_prefabs).AsSingle().NonLazy();
		}

		[System.Serializable]
		public class Prefabs {
			[SerializeField] private Blob blob;

			public Blob Blob => blob;
		}

	}
}
