using Game.Scripts.Helpers;
using Zenject;

namespace Game.Scripts.Installers
{
	public class HelpersInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<PlatformHelper>().AsSingle().NonLazy();
		}
	}
}
