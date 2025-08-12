using Core.Elements.Windows.Base.Data;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Elements.Windows.Files.Data.Base
{
	/// <summary>
	/// Устаревшее название - "FileMaterial"
	/// </summary>
	[MaterialModel(MaterialModel.File)]
	[MaterialDataLoader("/files")]
	public abstract class FileMaterialData : WindowMaterialData
	{
		[MaterialDataPropertyParse("file/path"), MaterialDataPropertyUpdate]
		public string Path { get; set; }
		
		[MaterialDataPropertyParse("file/url"), MaterialDataPropertyUpdate]
		public string Url { get; set; }
		
		[MaterialDataPropertyParse("file/mimetype"), MaterialDataPropertyUpdate]
		public string MimeType { get; set; }

		public FileMaterialData() => Model = MaterialModel.File;
	}
}