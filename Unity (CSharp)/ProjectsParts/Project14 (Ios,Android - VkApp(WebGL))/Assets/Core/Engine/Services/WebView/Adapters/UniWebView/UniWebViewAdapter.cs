using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.WVService
{
#if WEBVIEW_SERVICE
	public class UniWebViewAdapter : MonoBehaviour, IWebViewAdapter
	{
		private UniWebView _webView;

		private void Awake()
		{
			_webView = GetComponent<UniWebView>();
		}
		public void SetUrl(string url)
		{
			_webView.Load(url);
		}
	}
#endif

}