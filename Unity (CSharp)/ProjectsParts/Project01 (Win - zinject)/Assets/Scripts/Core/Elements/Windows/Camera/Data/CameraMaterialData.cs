using Core.Elements.Windows.Video.Data.Base;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.Camera.Data
{
	/// <summary>
	/// Устаревшее название - "CameraMaterial"
	/// </summary>
	[MaterialDataLoader("/cameras")]
	public class CameraMaterialData : VideoMaterialData
	{
		[MaterialDataPropertyParse("url"), MaterialDataPropertyUpdate]
		public string CameraUrl { get; set; }
		
		[MaterialDataPropertyParse("position"), MaterialDataPropertyUpdate] 
		public string Location { get; set; }
		
		public CameraMaterialData() => Category = MaterialCategory.Camera;
	}
}