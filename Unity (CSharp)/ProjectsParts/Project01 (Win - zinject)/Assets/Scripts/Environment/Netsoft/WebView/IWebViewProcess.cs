using Cysharp.Threading.Tasks;
using Environment.Netsoft.WebView.Settings;

namespace Environment.Netsoft.WebView
{
	public interface IWebViewProcess
	{
		public UniTask<bool> CreateProcess(IApplicationRunOptions arguments);

		public void TerminateProcess();
	}
}