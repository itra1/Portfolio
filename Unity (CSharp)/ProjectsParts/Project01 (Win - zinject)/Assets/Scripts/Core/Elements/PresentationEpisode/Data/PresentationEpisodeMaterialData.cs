using Core.Elements.PresentationEpisode.Data.Parsing;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.PresentationEpisode.Data
{
	/// <summary>
	/// Устаревшее название - "EpisodePresentationMaterial"
	/// </summary>
	[MaterialDataLoader("/episode")]
	[MaterialDataParser(typeof(PresentationEpisodeMaterialDataParser))]
	public class PresentationEpisodeMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("order"), MaterialDataPropertyUpdate]
		public int Order { get; set; }

		[MaterialDataPropertyParse("speaker"), MaterialDataPropertyUpdate]
		public string Speaker { get; set; }
		
		[MaterialDataPropertyParse("isAutostartVideo"), MaterialDataPropertyUpdate]
		public bool IsAutostartVideo { get; set; }
		
		[MaterialDataPropertyParse("isShowTimer"), MaterialDataPropertyUpdate]
		public bool IsShowTimer { get; set; }
		
		[MaterialDataPropertyParse("light"), MaterialDataPropertyUpdate]
		public string Light { get; set; }
		
		[MaterialDataPropertyParse("music"), MaterialDataPropertyUpdate]
		public string Music { get; set; }
		
		[MaterialDataPropertyParse("interactive"), MaterialDataPropertyUpdate]
		public bool? Interactive { get; set; }
		
		[MaterialDataPropertyParse("timing"), MaterialDataPropertyUpdate]
		public float Timing { get; set; }
		
		[MaterialDataPropertyParse("text"), MaterialDataPropertyUpdate]
		public string Text { get; set; }
		
		[MaterialDataPropertyParse("todo"), MaterialDataPropertyUpdate]
		public string Todo { get; set; }
		
		public override string UpdateMessageType => MessageType.PresentationEpisodeMaterialDataUpdate;

		public PresentationEpisodeMaterialData()
		{
			Model = MaterialModel.Episode;
		}
	}
}