using Core.Elements.Windows.Browser.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.Gis.Data
{
	/// <summary>
	/// Устаревшее название - "GisMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.Gis)]
	[MaterialDataLoader("/gis")]
	public class GisMaterialData : BrowserMaterialData
	{
		[MaterialDataPropertyParse("order"), MaterialDataPropertyUpdate]
		public int Order { get; set; }
		
		[MaterialDataPropertyParse("fullName"), MaterialDataPropertyUpdate]
		public string FullName { get; set; }
		
		[MaterialDataPropertyParse("FOIV"), MaterialDataPropertyUpdate]
		public string FOIV { get; set; }
		
		[MaterialDataPropertyParse("fullFOIV"), MaterialDataPropertyUpdate]
		public string FullFOIV { get; set; }
		
		public GisMaterialData()
		{
			Model = MaterialModel.Gis;
			Category = MaterialCategory.Gis;
		}
	}
}