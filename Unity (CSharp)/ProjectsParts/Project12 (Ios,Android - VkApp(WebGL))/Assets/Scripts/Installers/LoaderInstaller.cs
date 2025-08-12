using Game.Scripts.App;
using Game.Scripts.Controllers.Tutorials;
using Game.Scripts.Managers;
using Zenject;

namespace Game.Scripts.Installers
{
	class LoaderInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<TutorialController>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ApplicationLoaderHelper>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<ApplicationRun>().AsSingle().NonLazy();
		}
	}
}
