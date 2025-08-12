using System.Collections.Generic;
using Core.Materials.Attributes;
using Core.Workers.Material;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "AreaMaterial"
	/// </summary>
	[MaterialDataWorker(typeof(AreaMaterialDataWorker))]
	public class AreaMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("parentAreaId"), MaterialDataPropertyUpdate]
		public ulong? ParentId { get; set; }
		
		[MaterialDataPropertyParse("childrenAreasIds"), MaterialDataPropertyUpdate]
		public List<ulong> ChildIds { get; set; } = new ();
		
		[MaterialDataPropertyParse("colNum"), MaterialDataPropertyUpdate]
		public int Column { get; set; }
		
		[MaterialDataPropertyParse("rowNum"), MaterialDataPropertyUpdate]
		public int Row { get; set; }
		
		[MaterialDataPropertyParse("sizeX"), MaterialDataPropertyUpdate]
		public float SizeX { get; set; }
		
		[MaterialDataPropertyParse("sizeY"), MaterialDataPropertyUpdate]
		public float SizeY { get; set; }
		
		public List<AreaMaterialData> Children { get; set; } = new ();
	}
}