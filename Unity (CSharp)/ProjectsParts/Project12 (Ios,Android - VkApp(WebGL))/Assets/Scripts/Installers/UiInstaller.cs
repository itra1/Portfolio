using Game.Scripts.UI;
using Game.Scripts.UI.Controllers.Factorys;
using Game.Scripts.UI.Presenters.Factorys;
using Zenject;

namespace Game.Scripts.Installers
{
	public class UiInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			_ = Container.BindInterfacesTo<WindowPresenterFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<WindowPresenterControllerFactory>().AsSingle().NonLazy();
			_ = Container.BindInterfacesTo<UiNavigator>().AsSingle().NonLazy();
		}
	}
}
