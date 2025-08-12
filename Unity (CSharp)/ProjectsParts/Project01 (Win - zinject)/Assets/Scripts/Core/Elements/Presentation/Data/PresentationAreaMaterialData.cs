using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.Presentation.Data
{
	/// <summary>
	/// Устаревшее название - "PresentationAreaMaterial"
	/// </summary>
	[MaterialDataLoader("/presentation-area")]
	public class PresentationAreaMaterialData : AreaMaterialData
	{
		[MaterialDataPropertyParse("presentationId"), MaterialDataPropertyUpdate]
		public ulong PresentationId { get; set; }
		
		[MaterialDataPropertyParse("path"), MaterialDataPropertyUpdate]
		public TimerSound TimerSound { get; set; }
		
		public override string UpdateMessageType => MessageType.PresentationAreaMaterialDataUpdate;
		
		public PresentationAreaMaterialData() => Model = MaterialModel.MasterMeeting;
	}
}