using Game.Scripts.Providers.Platforms;
using Zenject;

namespace Game.Scripts.Installers
{
	public class PlatformInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<PlatformsProvider>().AsSingle().NonLazy();
		}
	}
}
