using Core.Elements.Windows.WebView.Data;
using Cysharp.Threading.Tasks;
using Environment.Netsoft.WebView.Data;
using UnityEngine.Events;

namespace Environment.Netsoft.WebView
{
	public interface IWebViewApplication
	{
		string Id { get; }
		bool Connected { get; }
		WebViewStateData StateData { get; }
		UnityEvent OnApplicationRestart { get; }
		UnityEvent<WebViewStateData> OnStateChange { get; }

		void Create(INsWebViewMaterialData materialData, WebViewState stateData);
		UniTask<bool> RunApplication();
		void Send(string action, object data = null, UnityAction<string, object> OnComplete = null);
		void Close();
	}
}
