using Cysharp.Threading.Tasks;

using Providers.Network.Common;
using Providers.User;
using Providers.WebView.Common;

using UnityEngine;

using Zenject;

namespace Providers.WebView
{
	public class WebViewProvider : IWebViewProvider, ITickable
	{
		private IUserProvider _user;
		private IWebViewAdapter _uniWebView;
		private INetworkSettings _networkSettings;
		private GameObject _prefab;
		private bool _isInit = false;
		private int _lastWidth = 0;

		public WebViewProvider(IUserProvider user, INetworkSettings networdSettings)
		{
			_user = user;
			_networkSettings = networdSettings;
			_prefab = Resources.Load<GameObject>("WebView/UniWebView");
		}

		public async UniTask Init()
		{
			Screen.orientation = ScreenOrientation.AutoRotation;
			_isInit = true;
			_uniWebView = MonoBehaviour.Instantiate(_prefab).GetComponent<IWebViewAdapter>();

			await UniTask.Delay(500);
			_uniWebView.OpenUrl(_networkSettings.ServerDev + $"/auth/{_user.Token}");

		}

		public void Tick()
		{
			if (!_isInit) return;

			if (_lastWidth != Screen.width)
			{
				_lastWidth = Screen.width;

				if (Screen.width > Screen.height)
					_uniWebView.SetFrame(new Rect(0, 0, Screen.width, Screen.height));
				else
					_uniWebView.SetFrame(new Rect(0, Screen.height * 0.04f, Screen.width, Screen.height * 0.96f));
			}

		}
	}
}
