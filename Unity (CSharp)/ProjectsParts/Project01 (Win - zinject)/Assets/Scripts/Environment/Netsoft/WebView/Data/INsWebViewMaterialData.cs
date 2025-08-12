using Core.Elements.Windows.WebView.Data;

namespace Environment.Netsoft.WebView.Data
{
	public interface INsWebViewMaterialData
	{
		public string Url { get; set; }
		public string MaterialType { get; set; }
		public string SubType { get; set; }
		public WebViewAuthData AuthData { get; set; }
	}
}
