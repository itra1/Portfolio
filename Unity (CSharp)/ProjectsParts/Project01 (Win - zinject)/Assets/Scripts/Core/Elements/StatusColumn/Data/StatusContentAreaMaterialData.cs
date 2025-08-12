using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.StatusColumn.Data
{
	/// <summary>
	/// Устаревшее название - "StatusContentAreaMaterial"
	/// </summary>
	[MaterialDataLoader("/status-content-area")]
	public class StatusContentAreaMaterialData : AreaMaterialData
	{
		[MaterialDataPropertyParse("statusContent"), MaterialDataPropertyUpdate]
		public StatusContentMaterialData StatusContent { get; set; }
		
		public override string UpdateMessageType => MessageType.StatusContentAreaMaterialDataUpdate;
		
		public StatusContentAreaMaterialData() => Model = MaterialModel.StatusContent;
	}
}