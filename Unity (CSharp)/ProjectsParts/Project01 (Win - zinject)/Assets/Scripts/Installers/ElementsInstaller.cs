using Core.Installers.Base;
using Elements.ScreenModes.Controller;
using Elements.Status.Controller.Factory;
using Elements.StatusColumn.Controller.Factory;
using Elements.Statuses.Controller;
using Elements.StatusTabItem.Controller.Factory;
using Elements.StatusTabs.Controller.Factory;
using Elements.Windows.Common.Controller.Factory;
using Elements.Windows.Common.Presenter.Factory;
using UI.Elements.Controller;
using UI.ShadedElements.Controller;

namespace Installers
{
	public class ElementsInstaller : AutoResolvingMonoInstaller<ElementsInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<WindowControllerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<WindowPresenterFactory>().AsSingle().NonLazy();
			
			BindInterfacesTo<StatusTabItemControllerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<StatusTabsControllerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<StatusColumnControllerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<StatusControllerFactory>().AsSingle().NonLazy();
			Bind<IStatusesController>().To<StatusesController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ScreenModesController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ShadedScreenModesController>().AsSingle().NonLazy();
			BindInterfacesTo<ShadedFloatingWindowsController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ElementsController>().AsSingle().NonLazy();
			
			base.InstallBindings();
		}
		
		protected override void ResolveAll()
		{
			Resolve<IWindowControllerFactory>();
			Resolve<IWindowPresenterFactory>();
			
			Resolve<IStatusTabItemControllerFactory>();
			Resolve<IStatusTabsControllerFactory>();
			Resolve<IStatusColumnControllerFactory>();
			Resolve<IStatusControllerFactory>();
			Resolve<IStatusesController>();
			
			Resolve<IScreenMode>();
			Resolve<IScreenModesController>();
			
			Resolve<IShadedScreenModesController>();
			Resolve<IShadedFloatingWindowsController>();
			
			Resolve<IElementsController>();
			
			base.ResolveAll();
		}
	}
}