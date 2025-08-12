using Base.Presenter;
using Core.Installers.Base;
using Elements.Common.Presenter.Factory;
using UI.Audio;
using UI.Audio.Controller;
using UI.Canvas.Controller;
using UI.Canvas.Presenter;
using UI.Console;
using UI.FullScreen.Presenter;
using UI.LoadingIndicator.Controller;
using UI.MouseCursor.Controller;
using UI.MouseCursor.Controller.Factory;
using UI.MouseCursor.Presenter;
using UI.MusicPlayer.Controller;
using UI.MusicPlayer.Controller.Converter;
using UI.Notifications.Controller;
using UI.Notifications.Presenter.Popups.Factory;
using UI.Prefabs;
using UI.Profiling.Controller;
using UI.ScreenBlocker.Controller;
using UI.ScreenBlocker.Presenter.Popups.Common.Factory;
using UI.ScreenLayers.Controller;
using UI.ScreenLayers.Presenter;
using UI.SplashScreens.Background.Controller;
using UI.SplashScreens.Intro.Controller;
using UI.SplashScreens.Screensaver.Controller;
using UI.Switches;
using UI.Switches.Triggers.Factory;
using UI.Timers.Controller;
using UI.Timers.Controller.Items.Factory;

namespace Installers
{
	public class UIInstaller : AutoResolvingMonoInstaller<UIInstaller>
	{
		public override void InstallBindings()
		{
			BindInterfacesTo<PrefabProvider>().AsSingle().NonLazy();
			
			BindInterfacesTo<PresenterFactory>().AsSingle().NonLazy();
			
			BindInterfacesTo<TriggerFactory>().AsSingle().NonLazy();
			BindInterfacesTo<TriggerSwitch>().AsSingle().NonLazy();
			
			BindInterfacesTo<AudioMixerGroupProvider>().AsSingle().NonLazy();
			BindInterfacesTo<AudioController>().AsSingle().NonLazy();
			
			BindInterfacesTo<MouseCursorTextureFactory>().AsSingle().NonLazy();
			BindInterfacesTo<MouseCursorController>().AsSingle().NonLazy();
			BindInterfacesTo<MouseCursorPresenter>().AsSingle().NonLazy();
			
			BindInterfacesTo<LoadingIndicatorController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ProfilingController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ConsoleSpawner>().AsSingle().NonLazy();
			
			BindInterfacesTo<AudioClipInfoResolver>().AsSingle().NonLazy();
			BindInterfacesTo<MusicPlayerController>().AsSingle().NonLazy();
			
			BindInterfacesTo<TimersFactory>().AsSingle().NonLazy();
			BindInterfacesTo<TimersController>().AsSingle().NonLazy();
			
			BindInterfacesTo<IntroController>().AsSingle().NonLazy();
			
			BindInterfacesTo<BackgroundController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ScreensaverController>().AsSingle().NonLazy();
			
			BindInterfacesTo<NotificationPopupFactory>().AsSingle().NonLazy();
			BindInterfacesTo<NotificationsController>().AsSingle().NonLazy();
			
			BindInterfacesTo<ScreenBlockerPopupFactory>().AsSingle().NonLazy();
			BindInterfacesTo<ScreenBlockersController>().AsSingle().NonLazy();
			
			Bind(typeof(ICanvasPresenter), typeof(ICanvasModifying), typeof(ICanvasRectTransform), typeof(INonRenderedContainer))
				.FromInstance(FindObjectOfType<CanvasPresenter>()).AsSingle().NonLazy();
			BindInterfacesTo<CanvasController>().AsSingle().NonLazy();
			
			Bind<IScreenLayersPresenter>().FromInstance(FindObjectOfType<ScreenLayersPresenter>()).AsSingle().NonLazy();
			BindInterfacesTo<ScreenLayersController>().AsSingle().NonLazy();
			
			BindInterfacesTo<WindowFullScreenPresenter>()
				.FromInstance(FindObjectOfType<WindowFullScreenPresenter>()).AsSingle().NonLazy();
			
			BindInterfacesTo<WidgetFullScreenPresenter>()
				.FromInstance(FindObjectOfType<WidgetFullScreenPresenter>()).AsSingle().NonLazy();
			
			base.InstallBindings();
		}
		
		protected override void ResolveAll()
		{
			Resolve<IPrefabProvider>();
			
			Resolve<IPresenterFactory>();
			
			Resolve<ITriggerFactory>();
			Resolve<ITriggerSwitch>();
			
			Resolve<IAudioMixerGroupProvider>();
			Resolve<IAudioController>();
			
			Resolve<ICustomTriggerSwitch>();
			
			Resolve<IMouseCursorTextureFactory>();
			Resolve<IMouseCursorController>();
			Resolve<IMouseCursorPresenter>();
			
			Resolve<ILoadingIndicatorController>();
			
			Resolve<IProfilingController>();
			
			Resolve<IConsoleSpawner>();
			
			Resolve<IAudioClipInfoResolver>();
			Resolve<IMusicPlayerController>();
			
			Resolve<ITimersFactory>();
			Resolve<ITimersController>();
			
			Resolve<IIntroController>();
			
			Resolve<IBackgroundController>();
			
			Resolve<IScreensaverController>();
			
			Resolve<INotificationPopupFactory>();
			Resolve<INotificationsController>();
			
			Resolve<IScreenBlockerPopupFactory>();
			Resolve<IScreenBlockersController>();
			
			Resolve<ICanvasPresenter>();
			Resolve<ICanvasModifying>();
			Resolve<INonRenderedContainer>();
			Resolve<ICanvasController>();
			
			Resolve<IScreenLayersPresenter>();
			Resolve<IScreenLayersController>();
			
			Resolve<IWindowFullScreenToggle>();
			
			Resolve<IWidgetFullScreenToggle>();
			
			base.ResolveAll();
		}
	}
}