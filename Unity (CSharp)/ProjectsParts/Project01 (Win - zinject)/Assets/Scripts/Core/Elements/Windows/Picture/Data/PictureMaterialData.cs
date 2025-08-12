using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.Files.Data.Base;
using Core.Materials.Attributes;

namespace Core.Elements.Windows.Picture.Data
{
	/// <summary>
	/// Устаревшее название - "PictureMaterial"
	/// </summary>
	[WindowState(typeof(PictureState))]
	public class PictureMaterialData : FileMaterialData
	{
		[MaterialDataPropertyParse("view"), MaterialDataPropertyUpdate]
		public string View { get; set; }
	}
}