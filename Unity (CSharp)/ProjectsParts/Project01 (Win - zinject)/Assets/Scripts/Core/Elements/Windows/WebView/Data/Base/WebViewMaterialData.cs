using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.NSWebView.Data.Interfaces;
using Core.Materials.Attributes;
using Core.Workers.Material;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.WebView.Data.Base
{
	[MaterialDataWorker(typeof(BrowserMaterialDataWorker))]
	[WindowState(typeof(WebViewState))]
	public abstract class WebViewMaterialData : WindowMaterialData, INsWvWindowMaterialData
	{
		[MaterialDataPropertyParse("url"), MaterialDataPropertyUpdate]
		public string Url { get; set; }

		[MaterialDataPropertyParse("zoom"), MaterialDataPropertyUpdate]
		public int Zoom { get; set; }

		[MaterialDataPropertyParse("options"), MaterialDataPropertyUpdate]
		public JSON Options { get; set; }

		[MaterialDataPropertyParse("authData"), MaterialDataPropertyUpdate]
		public WebViewAuthData AuthData { get; set; }

		public virtual bool WindowOffice => false;
		public virtual string WindowUrl => Url;

		public string WindowMaterialType => this.MaterialType;

		public string WindowSubType => this.SubType;

		public WebViewAuthData WindowAuthData => this.AuthData;
	}
}