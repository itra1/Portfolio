using UnityEngine;

namespace Providers.WebView.Common
{
	public interface IWebViewAdapter
	{
		void SetHeader(string key, string value);
		void OpenUrl(string url);
		void SetFrame(Rect rect);
	}
}
