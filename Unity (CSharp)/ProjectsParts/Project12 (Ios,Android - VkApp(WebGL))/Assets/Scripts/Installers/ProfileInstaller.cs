using Game.Scripts.Providers.Profiles;
using Game.Scripts.Providers.Profiles.Handlers;
using Zenject;

namespace Game.Scripts.Installers
{
	public class ProfileInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<ProfileProvider>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ProfileLevelHandler>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ProfileStarsHandler>().AsSingle().NonLazy();
		}
	}
}
