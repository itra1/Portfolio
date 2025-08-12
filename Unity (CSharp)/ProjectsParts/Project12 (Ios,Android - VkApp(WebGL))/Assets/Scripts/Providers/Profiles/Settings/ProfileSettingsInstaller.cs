using UnityEngine;
using Zenject;

namespace Game.Scripts.Providers.Profiles.Settings
{
	[CreateAssetMenu(fileName = "ProfileSettingsInstaller", menuName = "Installers/ProfileSettings")]
	public class ProfileSettingsInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private ProfileSettings _profileSettings;

		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<ProfileSettings>().FromInstance(_profileSettings).AsSingle().NonLazy();
		}
	}
}
