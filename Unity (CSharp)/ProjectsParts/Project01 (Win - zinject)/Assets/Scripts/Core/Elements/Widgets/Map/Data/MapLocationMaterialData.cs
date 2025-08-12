using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;

namespace Core.Elements.Widgets.Map.Data
{
	/// <summary>
	/// Устаревшее название - "MapDataLocationMaterial"
	/// </summary>
	[MaterialDataLoader("/locations")]
	public class MapLocationMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("dataLayerId"), MaterialDataPropertyUpdate]
		public int DataLayerId { get; set; }
		
		[MaterialDataPropertyParse("minVisibilityZoom"), MaterialDataPropertyUpdate]
		public float MinVisibilityZoom { get; set; }
		
		[MaterialDataPropertyParse("maxVisibilityZoom"), MaterialDataPropertyUpdate]
		public float MaxVisibilityZoom { get; set; }
		
		[MaterialDataPropertyParse("dataLayer/layerColor"), MaterialDataPropertyUpdate]
		public string Color { get; set; }
		
		[MaterialDataPropertyParse("alpha"), MaterialDataPropertyUpdate]
		public float Alpha { get; set; }
		
		[MaterialDataPropertyParse("geography"), MaterialDataPropertyUpdate]
		public GeographyPoint GeographyPoint { get; set; }
		
		public MapLocationMaterialData()
		{
			Model = MaterialModel.MapDataLocation;
		}
	}
}