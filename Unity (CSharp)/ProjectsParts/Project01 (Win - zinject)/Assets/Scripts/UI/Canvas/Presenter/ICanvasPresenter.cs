using UI.LoadingIndicator.Presenter;
using UI.MusicPlayer.Presenter;
using UI.Notifications.Presenter;
using UI.Profiling.Presenter.Base;
using UI.ScreenBlocker.Presenter;
using UI.ScreenLayers.Presenter;
using UI.SplashScreens.Background.Presenter;
using UI.SplashScreens.Intro.Presenter;
using UI.SplashScreens.Screensaver.Presenter;
using UI.Timers.Presenter;
using UnityEngine;

namespace UI.Canvas.Presenter
{
	public interface ICanvasPresenter : ICanvasRectTransform
	{
		float CanvasPlaneDistance { get; }
		
		RectTransform Elements { get; }
		IIntroPresenter Intro { get; }
		IBackgroundPresenter Background { get; }
		IScreenLayersPresenter ScreenLayers { get; }
		IScreensaverPresenter Screensaver { get; }
		INotificationsPresenter Notifications { get; }
		IScreenBlockersPresenter ScreenBlockers { get; }
		ITimersPresenter Timers { get; }
		IMusicPlayerPresenter MusicPlayer { get; }
		ILoadingIndicatorPresenter LoadingIndicator { get; }
		IProfilerItemPresenter FpsCounter { get; }
		
		GameObject ConsolePrefab { get; }
	}
}