using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.Browser.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.OfficeDocument.Data.Web
{
	/// <summary>
	/// Устаревшее название - "ViewMaterial"
	/// </summary>
	[WindowState(typeof(WebOfficeDocumentState))]
	public class WebOfficeDocumentMaterialData : BrowserMaterialData
	{
		[MaterialDataPropertyParse("view")]
		public string View { get; set; }

		public WebOfficeDocumentMaterialData() => Model = MaterialModel.WebOfficeDocument;

		public override bool WindowOffice => true;
		public override string WindowUrl => this.View;
	}
}