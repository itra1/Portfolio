using Core.Elements.Windows.WebView.Data.Base;
using Core.Materials.Attributes;

namespace Core.Elements.Windows.NSWebView.Data
{
	public abstract class NsWvOfficeMaterialData : WebViewMaterialData
	{
		[MaterialDataPropertyParse("view")]
		public string View { get; set; }

		public override bool WindowOffice => true;
		public override string WindowUrl => this.View;
	}
}
