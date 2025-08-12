using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Materials.Data;
using Core.Messages;

namespace Core.Elements.PresentationEpisode.Data
{
	/// <summary>
	/// Устаревшее название - "EpisodePresentationAreaMaterial"
	/// </summary>
	[MaterialDataLoader("/episodes")]
	public class PresentationEpisodeAreaMaterialData : AreaMaterialData
	{
		[MaterialDataPropertyParse("episodeId"), MaterialDataPropertyUpdate]
		public ulong EpisodeId { get; set; }
		
		public override string UpdateMessageType => MessageType.PresentationEpisodeAreaMaterialDataUpdate;
		
		public PresentationEpisodeAreaMaterialData()
		{
			Model = MaterialModel.EpisodeArea;
			Category = MaterialCategory.Episode;
		}
	}
}