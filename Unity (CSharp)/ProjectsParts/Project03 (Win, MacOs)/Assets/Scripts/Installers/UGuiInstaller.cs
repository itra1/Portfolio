
using Settings.Themes;

using UGui.Popups;
using UGui.Screens;
using UGui.Screens.Factorys;

using Zenject;

namespace Installers
{
	public class UGuiInstaller: MonoInstaller
	{

		public override void InstallBindings()
		{
			Container.Bind<ThemeProvider>().AsSingle().NonLazy();
			Container.Bind<ScreenFactory>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<PopupProvider>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<ScreenProvider>().AsSingle().NonLazy();
		}

		private void Awake()
		{
			Container.Resolve<IPopupProvider>();
			Container.Resolve<IScreenProvider>();
		}

	}
}
