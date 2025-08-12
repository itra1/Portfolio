using Components.Pipes;
using Pipes;

namespace Environment.Netsoft.WebView
{
	public class WebViewPipeServer : IWebViewPipeServer
	{
		private IPipeServer _pipeServer;
		public WebViewPipeServer(IPipeServerFactory pipeServerFactory)
		{
			_pipeServer = pipeServerFactory.Create();
		}
	}
}
