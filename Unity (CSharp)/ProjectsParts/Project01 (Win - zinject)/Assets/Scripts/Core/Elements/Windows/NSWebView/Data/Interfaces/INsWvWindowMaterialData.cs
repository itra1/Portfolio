using Core.Elements.Windows.WebView.Data;

namespace Core.Elements.Windows.NSWebView.Data.Interfaces
{
	public interface INsWvWindowMaterialData
	{
		public bool WindowOffice { get; }
		public string WindowUrl { get; }
		public string WindowMaterialType { get; }
		public string WindowSubType { get; }
		public WebViewAuthData WindowAuthData { get; }
	}
}
