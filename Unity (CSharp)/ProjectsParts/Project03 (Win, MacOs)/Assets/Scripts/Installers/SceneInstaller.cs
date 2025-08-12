using Providers.App;
using Zenject;

namespace Installers
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<AppRunner>().AsSingle().NonLazy();

			ResolveAll();
		}

		private void ResolveAll()
		{
		}

	}
}
