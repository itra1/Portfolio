using System.Collections.Generic;
using Core.Elements.Widgets.Map.Data.Parser;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Workers.Material;

namespace Core.Elements.Widgets.Map.Data
{
	/// <summary>
	/// Устаревшее название - "MapDataLayerMaterial"
	/// </summary>
	[MaterialDataLoader("/data-layers")]
	[MaterialDataParser(typeof(MapLayerMaterialDataParser))]
	[MaterialDataWorker(typeof(MapLayerMaterialDataWorker))]
	public class MapLayerMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("layerColor"), MaterialDataPropertyUpdate]
		public string LayerColor { get; set; }
		
		[MaterialDataPropertyParse("locationsIds"), MaterialDataPropertyUpdate]
		public ulong[] LocationIds { get; set; }
		
		[MaterialDataPropertyUpdate]
		public List<MapLocationMaterialData> Locations { get; set; } = new ();
		
		public MapLayerMaterialData()
		{
			Model = MaterialModel.MapData;
		}
	}
}