using Core.Elements.Windows.Files.Data.Base;
using Core.Materials.Attributes;
using Leguar.TotalJSON;

namespace Core.Elements.Windows.Video.Data.Base
{
	/// <summary>
	/// Устаревшее название - "VideoMaterial"
	/// </summary>
	public abstract class VideoMaterialData : FileMaterialData
	{
		[MaterialDataPropertyParse("options"), MaterialDataPropertyUpdate]
		public JSON Options { get; set; }
	}
}