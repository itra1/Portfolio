using Core.Installers.Base;
using Elements.Desktop.Controller.Factory;
using Elements.Desktops.Controller;
using Elements.DesktopWidgetArea.Controller.Factory;
using Elements.FloatingWindow.Controller.Factory;
using Elements.FloatingWindow.Presenter.Factory;
using Elements.FloatingWindow.Presenter.WindowAdapters.Common.Factory;
using Elements.FloatingWindows.Controller;
using Elements.Presentation.Controller.Factory;
using Elements.PresentationEpisode.Controller.Factory;
using Elements.PresentationEpisodeScreen.Controller.Factory;
using Elements.Presentations.Controller;
using Elements.Widgets.Common.Controller.Factory;
using Elements.Widgets.Common.Presenter.Factory;

namespace Installers
{
    public class ExtendedElementsInstaller : AutoResolvingInstaller
    {
        public override void InstallBindings()
        {
            BindInterfacesTo<WidgetControllerFactory>().AsSingle().NonLazy();
            BindInterfacesTo<WidgetPresenterFactory>().AsSingle().NonLazy();
            
            BindInterfacesTo<DesktopWidgetAreaControllerFactory>().AsSingle().NonLazy();
            BindInterfacesTo<DesktopControllerFactory>().AsSingle().NonLazy();
            Bind<IDesktopsController>().To<DesktopsController>().AsSingle().NonLazy();
            
            BindInterfacesTo<PresentationEpisodeScreenControllerFactory>().AsSingle().NonLazy();
            BindInterfacesTo<PresentationEpisodeControllerFactory>().AsSingle().NonLazy();
            BindInterfacesTo<PresentationControllerFactory>().AsSingle().NonLazy();
            Bind<IPresentationsController>().To<PresentationsController>().AsSingle().NonLazy();
            
            BindInterfacesTo<WindowPresenterAdapterFactory>().AsSingle().NonLazy();
            
            BindInterfacesTo<FloatingWindowPresenterFactory>().AsSingle().NonLazy();
            BindInterfacesTo<FloatingWindowControllerFactory>().AsSingle().NonLazy();
            BindInterfacesTo<FloatingWindowsController>().AsSingle().NonLazy();
            
            base.InstallBindings();
        }
        
        protected override void ResolveAll()
        {
            Resolve<IWidgetControllerFactory>();
            Resolve<IWidgetPresenterFactory>();
            
            Resolve<IDesktopWidgetAreaControllerFactory>();
            Resolve<IDesktopControllerFactory>();
            Resolve<IDesktopsController>();
            
            Resolve<IPresentationEpisodeScreenControllerFactory>();
            Resolve<IPresentationEpisodeControllerFactory>();
            Resolve<IPresentationControllerFactory>();
            Resolve<IPresentationsController>();
            
            Resolve<IWindowPresenterAdapterFactory>();
            
            Resolve<IFloatingWindowPresenterFactory>();
            Resolve<IFloatingWindowControllerFactory>();
            Resolve<IFloatingWindowsController>();
            
            base.ResolveAll();
        }
    }
}