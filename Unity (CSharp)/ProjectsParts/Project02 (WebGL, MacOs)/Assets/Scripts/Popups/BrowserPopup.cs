using System.Collections;
using UnityEngine;

namespace it.Popups
{
	public class BrowserPopup : PopupBase
	{
//#if UNITY_STANDALONE_WIN
//		[SerializeField] private Vuplex.WebView.CanvasWebViewPrefab _browser;
//		private Vuplex.WebView.CanvasWebViewPrefab _instBrowser;
//#endif

		private string _url;
		private string _html;
		private bool _isInit;

		protected override void EnableInit()
		{
			base.EnableInit();
//#if UNITY_STANDALONE_WIN
//			if (_instBrowser != null)
//				Destroy(_instBrowser.gameObject);
//			_isInit = false;

//			var go = Instantiate(_browser.gameObject, _browser.transform.parent);
//			_instBrowser = go.GetComponent<Vuplex.WebView.CanvasWebViewPrefab>();
//			go.gameObject.SetActive(true);
//			_instBrowser.Initialized += (e1, e2) =>
//			{
//				_isInit = true;

//				if(!string.IsNullOrEmpty(_html)){
//					_instBrowser.WebView.LoadHtml(_html);
//				}
//				_html = "";

//			};
//#endif
		}

		public void SetUrl(string url)
		{
			_url = url;
//#if UNITY_STANDALONE_WIN

//			if(!_isInit)
//			{
//				_instBrowser.InitialUrl = _url;
//			}else{
//				_instBrowser.WebView.Dispose();
//				_instBrowser.WebView.LoadUrl(_url);
//			}
//#else
			Application.OpenURL(url);
//#endif
		}

		public void SetHtml(string html)
		{
			_html = html;

			//_instBrowser.Initialized += (arg1, arg2) =>
			//{
			//	_instBrowser.WebView.LoadHtml(html);
			//};
		}

	}
}