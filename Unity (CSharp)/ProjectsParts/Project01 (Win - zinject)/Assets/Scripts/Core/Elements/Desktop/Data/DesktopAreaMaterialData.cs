using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Desktop.Data
{
	/// <summary>
	/// Устаревшее название - "DesktopAreaMaterial"
	/// </summary>
	[MaterialDataLoader("/desktop-area")]
	public class DesktopAreaMaterialData : AreaMaterialData
	{
		[MaterialDataPropertyParse("desktopId")] 
		public ulong? DesktopId { get; set; }
		
		public override string UpdateMessageType => MessageType.DesktopAreaMaterialDataUpdate;
		
		public DesktopAreaMaterialData()
		{
			Model = MaterialModel.DesktopArea;
		}
	}
}