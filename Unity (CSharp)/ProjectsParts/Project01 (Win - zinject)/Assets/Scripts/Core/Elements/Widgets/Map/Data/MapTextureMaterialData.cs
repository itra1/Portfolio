using Core.Elements.Widgets.Map.Data.Parser;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;

namespace Core.Elements.Widgets.Map.Data
{
	/// <summary>
	/// Устаревшее название - "MapTexturesMaterial"
	/// </summary>
	[MaterialDataLoader("/map-presets")]
	[MaterialDataParser(typeof(MapTextureMaterialDataParser))]
	public class MapTextureMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("file/mimetype"), MaterialDataPropertyUpdate]
		public string MimeType { get; set; }
		
		[MaterialDataPropertyParse("file/path"), MaterialDataPropertyUpdate]
		public string Path { get; set; }
		
		[MaterialDataPropertyParse("file/url"), MaterialDataPropertyUpdate]
		public string Url { get; set; }
		
		[MaterialDataPropertyParse("legend"), MaterialDataPropertyUpdate]
		public MapLegend Legend { get; set; }

		public MapTextureMaterialData()
		{
			Model = MaterialModel.MapTexture;
		}
	}
}