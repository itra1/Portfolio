using System.Collections.Generic;
using Core.Elements.Status.Data.Parsing;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Status.Data
{
	/// <summary>
	/// Устаревшее название - "StatusMaterial"
	/// </summary>
	[MaterialDataLoader("/status")]
	[MaterialDataParser(typeof(StatusMaterialDataParser))]
	public class StatusMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("columnCount"), MaterialDataPropertyUpdate]
		public int ColumnCount { get; set; }
		
		[MaterialDataPropertyParse("isActive"), MaterialDataPropertyUpdate]
		public bool IsActive { get; set; }
		
		[MaterialDataPropertyParse("playlistColumns"), MaterialDataPropertyUpdate]
		public List<int> PlaylistColumns { get; set; }
		
		public override string UpdateMessageType => MessageType.StatusMaterialDataUpdate;
		
		public StatusMaterialData() => Model = MaterialModel.Status;
	}
}