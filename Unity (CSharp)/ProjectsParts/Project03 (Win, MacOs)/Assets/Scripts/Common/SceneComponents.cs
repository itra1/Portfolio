using Providers.Splash.Common;
using Providers.Splash.Presenter;
using Providers.SystemMessage.Common;
using UGui.Screens.Elements;
using UnityEngine;

namespace Common
{
	public class SceneComponents : MonoBehaviour, IScreenParent, ISystemMessagesParent, ISplash
	{
		[SerializeField] private RectTransform _screensParent;
		[SerializeField] private RectTransform _systemMessagesParent;
		[SerializeField] private SplashPresenter _splashPresenter;

		public RectTransform ScreensParent => _screensParent;
		public RectTransform SystemMessagesParent => _systemMessagesParent;
		public SplashPresenter SplashPresenter => _splashPresenter;
	}
}
