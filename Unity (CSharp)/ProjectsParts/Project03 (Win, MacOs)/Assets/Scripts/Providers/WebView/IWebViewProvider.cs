
using Cysharp.Threading.Tasks;

namespace Providers.WebView
{
	public interface IWebViewProvider
	{
		UniTask Init();
	}
}
