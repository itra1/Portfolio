using Providers.WebView.Common;
using UnityEngine;

public class UniWebViewAdapter : MonoBehaviour, IWebViewAdapter
{
	[SerializeField] private UniWebView _webView;

	public void OpenUrl(string url)
	{
		_webView.Load(url);
	}

	public void SetFrame(Rect rect)
	{
		_webView.Frame = rect;
	}

	public void SetHeader(string key, string value)
	{
		_webView.SetHeaderField(key, value);
	}


}
