using System.Collections.Generic;
using Core.Materials.Attributes;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.StatusColumn.Data
{
	/// <summary>
	/// Устаревшее название - "StatusContentData"
	/// </summary>
	public class StatusContentMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("activeMaterialId"), MaterialDataPropertyUpdate]
		public ulong ActiveMaterialId { get; set; }
		
		[MaterialDataPropertyParse("order"), MaterialDataPropertyUpdate]
		public int Order { get; set; }
		
		[MaterialDataPropertyParse("column"), MaterialDataPropertyUpdate]
		public int Column { get; set; } //Starts at value 1, not 0
		
		[MaterialDataPropertyParse("statusId"), MaterialDataPropertyUpdate]
		public ulong StatusId { get; set; }
		
		[MaterialDataPropertyParse("contentOrder"), MaterialDataPropertyUpdate]
		public List<ulong> ContentOrder { get; set; } = new();
		
		public override string UpdateMessageType => MessageType.StatusContentMaterialDataUpdate;
	}
}