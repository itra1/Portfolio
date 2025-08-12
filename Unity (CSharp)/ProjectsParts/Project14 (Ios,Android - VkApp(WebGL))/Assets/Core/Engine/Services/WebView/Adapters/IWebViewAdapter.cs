using System.Collections;

using UnityEngine;

namespace Core.Engine.Services.WVService
{
#if WEBVIEW_SERVICE
	public interface IWebViewAdapter
	{

		public void SetUrl(string url);

	}
#endif
}