using Core.Elements.Windows.WebView.Data.Base;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.Browser.Data
{
	/// <summary>
	/// Устаревшее название - "BrowserMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.Browser)]
	[MaterialDataLoader("/browsers")]
	public class BrowserMaterialData : WebViewMaterialData
	{
		
	}
}