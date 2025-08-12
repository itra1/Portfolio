using UnityEngine;
using Core.Engine.Services.WVService;

using Zenject;

namespace Core.Engine.Services.WVService
{
#if WEBVIEW_SERVICE
	public class WebViewService : IWebViewService, IInitializable
	{
		private IWebViewAdapter _viewPrefab;

		public void Initialize()
		{
			var go = Resources.Load<GameObject>("Services/UniWebView");
			_viewPrefab = go.GetComponent<IWebViewAdapter>();
		}
	}
#endif
}