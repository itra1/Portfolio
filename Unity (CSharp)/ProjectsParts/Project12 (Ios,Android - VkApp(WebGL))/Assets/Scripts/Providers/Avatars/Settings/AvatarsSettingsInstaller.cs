using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Avatars.Settings
{
	[CreateAssetMenu(fileName = "AvatarsSettingsInstaller", menuName = "Installers/AvatarsSettings")]
	public class AvatarsSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private AvatarsSettings _avatarsSettings;

		public AvatarsSettings AvatarsSettings => _avatarsSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInstance<AvatarsSettings>(_avatarsSettings).AsSingle().NonLazy();
		}

	}
}
