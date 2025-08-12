using Game.GameItems.Platforms;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Managers {
	[CreateAssetMenu(fileName = "GameSettings", menuName = "App/Game/GameSettings", order = 0)]
	public class GameSettings : ScriptableObjectInstaller {

		[SerializeField] private PlatformSettings _platformSettings;

		public PlatformSettings PlatformSettings => _platformSettings;

		public override void InstallBindings() {
			_ = Container.Bind<PlatformSettings>().FromInstance(_platformSettings);
		}

	}
}
