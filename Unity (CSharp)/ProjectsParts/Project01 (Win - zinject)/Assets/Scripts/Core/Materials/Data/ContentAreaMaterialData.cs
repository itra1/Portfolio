using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Messages;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "ContentAreaMaterial"
	/// </summary>
	[MaterialDataLoader("/content-areas")]
	public class ContentAreaMaterialData : AreaMaterialData
	{
		[MaterialDataPropertyParse("isContainer"), MaterialDataPropertyUpdate]
		public bool IsContainer { get; set; }
		
		[MaterialDataPropertyParse("cloneAlias"), MaterialDataPropertyUpdate]
		public string CloneAlias { get; set; }
		
		[MaterialDataPropertyParse("materialId"), MaterialDataPropertyUpdate]
		public ulong? MaterialId { get; set; }
		
		[MaterialDataPropertyParse("timeout"), MaterialDataPropertyUpdate]
		public ulong? TimeOut { get; set; }
		
		[MaterialDataPropertyParse("autoPlay"), MaterialDataPropertyUpdate]
		public bool? AutoPlay { get; set; }
		
		public int? Order { get; set; }
		
		public override string UpdateMessageType => MessageType.ContentAreaMaterialDataUpdate;
		
		public ContentAreaMaterialData()
		{
			Model = MaterialModel.Area;
			Category = MaterialCategory.Area;
		}
	}
}