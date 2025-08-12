using System.Collections.Generic;
using Core.Elements.Common.Data.Parsing;
using Core.Elements.Windows.Base.Data;
using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.Common.Data;
using Core.Elements.Windows.Video.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;
using Core.Workers.Material;

namespace Core.Elements.Windows.VideoSplit.Data
{
	/// <summary>
	/// Устаревшее название - "VideoSplitMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.VideoSplit)]
	[MaterialDataLoader("/videos")]
	[MaterialDataParser(typeof(MaterialDataParser<VideoSplitMaterialData>))]
	[WindowState(typeof(VideoState))]
	[MaterialDataWorker(typeof(VideoSplitMaterialDataWorker))]
	public class VideoSplitMaterialData : WindowMaterialData
	{
		[MaterialDataPropertyParse("files"), MaterialDataPropertyUpdate]
		public List<FileItemMaterialData> Files { get; set; }
		
		[MaterialDataPropertyParse("width"), MaterialDataPropertyUpdate]
		public int Width { get; set; }
		
		[MaterialDataPropertyParse("height"), MaterialDataPropertyUpdate]
		public int Height { get; set; }
		
		public VideoSplitMaterialData() => Model = MaterialModel.VideoSplit;
	}
}