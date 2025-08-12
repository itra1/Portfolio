namespace Environment.Netsoft.WebView
{
	public interface IWebViewFactory
	{
		IWebViewApplication Create();

		void Remove(IWebViewApplication application);
	}
}