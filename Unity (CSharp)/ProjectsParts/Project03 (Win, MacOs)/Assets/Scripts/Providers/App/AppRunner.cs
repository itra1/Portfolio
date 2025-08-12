using Cysharp.Threading.Tasks;
using Zenject;
using UGui.Screens;
using Providers.User;
using Providers.WebView;
using Providers.Splash;

namespace Providers.App
{
	public class AppRunner : IInitializable
	{
		private IScreenProvider _screenProvider;
		private IUserProvider _userProvider;
		private IWebViewProvider _webView;
		private ISplashProvider _splash;

		public AppRunner(IScreenProvider screenProvider, IUserProvider userProvider, IWebViewProvider webView, ISplashProvider splash)
		{
			_screenProvider = screenProvider;
			_userProvider = userProvider;
			_webView = webView;
			_splash = splash;
		}

		public void Initialize()
		{
			RunApp().Forget();
		}

		private async UniTask RunApp()
		{
			await _userProvider.Initialize();
			await _splash.Play();
			await _userProvider.RunUI();

			_webView.Init();

		}


	}
}
