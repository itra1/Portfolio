using Core.Elements.Desktop.Data.Parsing;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Desktop.Data
{
	/// <summary>
	/// Устаревшее название - "DesktopMaterial"
	/// </summary>
	[MaterialDataLoader("/desktop")]
	[MaterialDataParser(typeof(DesktopMaterialDataParser))]
	public class DesktopMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("userId"), MaterialDataPropertyUpdate]
		public ulong UserId { get; set; }
		
		[MaterialDataPropertyParse("areaId"), MaterialDataPropertyUpdate]
		public ulong AreaId { get; set; }
		
		[MaterialDataPropertyParse("isActive"), MaterialDataPropertyUpdate]
		public bool IsActive { get; set; }
		
		[MaterialDataPropertyParse("isFavorite"), MaterialDataPropertyUpdate]
		public bool IsFavorite { get; set; }
		
		public override string UpdateMessageType => MessageType.DesktopMaterialDataUpdate;
		
		public DesktopMaterialData()
		{
			Model = MaterialModel.Desktop;
		}
	}
}