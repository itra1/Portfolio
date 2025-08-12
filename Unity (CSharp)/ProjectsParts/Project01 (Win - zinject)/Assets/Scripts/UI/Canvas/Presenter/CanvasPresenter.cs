using Base.Presenter;
using com.ootii.Messages;
using Core.Messages;
using UI.LoadingIndicator.Presenter;
using UI.MusicPlayer.Presenter;
using UI.Notifications.Presenter;
using UI.Profiling.Presenter;
using UI.Profiling.Presenter.Base;
using UI.ScreenBlocker.Presenter;
using UI.ScreenLayers.Presenter;
using UI.SplashScreens.Background.Presenter;
using UI.SplashScreens.Intro.Presenter;
using UI.SplashScreens.Screensaver.Presenter;
using UI.Timers.Presenter;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Canvas.Presenter
{
	[RequireComponent(typeof(UnityEngine.Canvas), typeof(CanvasScaler))]
	public class CanvasPresenter : PresenterBase, ICanvasPresenter, ICanvasModifying, INonRenderedContainer
	{
		[SerializeField] private UnityEngine.Canvas _canvas;
		[SerializeField] private CanvasScaler _canvasScaler;
		[SerializeField] private RectTransform _elements;
		[SerializeField] private RectTransform _nonRenderedContent;
		[SerializeField] private IntroPresenter _intro;
		[SerializeField] private BackgroundPresenter _background;
		[SerializeField] private ScreenLayersPresenter _screenLayers;
		[SerializeField] private ScreensaverPresenter _screesaver;
		[SerializeField] private NotificationsPresenter _notifications;
		[SerializeField] private ScreenBlockersPresenter _screenBlockers;
		[SerializeField] private TimersPresenter _timers;
		[SerializeField] private MusicPlayerPresenter _musicPlayer;
		[SerializeField] private LoadingIndicatorPresenter _loadingIndicator;
		[SerializeField] private FpsCounterPresenter _fpsCounter;
		[SerializeField] private GameObject _concolePrefab;
		
		public float CanvasPlaneDistance => _canvas.planeDistance;
		
		public Vector2 ReferenceResolution
		{
			get => _canvasScaler.referenceResolution;
			set
			{
				_canvasScaler.referenceResolution = value;
				MessageDispatcher.SendMessage(MessageType.ReferenceResolutionSet);
			}
		}
		
		public RectTransform Elements => _elements;
		public RectTransform NonRenderedContent => _nonRenderedContent;
		public IIntroPresenter Intro => _intro;
		public IBackgroundPresenter Background => _background;
		public IScreenLayersPresenter ScreenLayers => _screenLayers;
		public IScreensaverPresenter Screensaver => _screesaver;
		public INotificationsPresenter Notifications => _notifications;
		public IScreenBlockersPresenter ScreenBlockers => _screenBlockers;
		public ITimersPresenter Timers => _timers;
		public IMusicPlayerPresenter MusicPlayer => _musicPlayer;
		public ILoadingIndicatorPresenter LoadingIndicator => _loadingIndicator;
		public IProfilerItemPresenter FpsCounter => _fpsCounter;
		
		public GameObject ConsolePrefab => _concolePrefab;
		
		[Inject]
		private void Initialize() => Visible = true;
	}
}