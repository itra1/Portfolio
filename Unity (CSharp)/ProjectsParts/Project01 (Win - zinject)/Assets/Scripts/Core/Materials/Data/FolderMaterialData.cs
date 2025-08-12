using System.Collections.Generic;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "FolderMaterial"
	/// </summary>
	[MaterialDataLoader("/folder")]
	public class FolderMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("children/id"), MaterialDataPropertyUpdate]
		public List<long> ChildIds { get; set; } = new ();
		
		[MaterialDataPropertyParse("parentId"), MaterialDataPropertyUpdate]
		public long? FolderParent { get; set; }
		
		public FolderMaterialData() => Model = MaterialModel.Folder;
	}
}