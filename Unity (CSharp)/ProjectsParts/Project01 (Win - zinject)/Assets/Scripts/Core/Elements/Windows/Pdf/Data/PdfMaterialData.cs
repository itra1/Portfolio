using Core.Elements.Windows.Base.Data.Attributes;
using Core.Elements.Windows.Files.Data.Base;
using Core.Materials.Attributes;
using Core.Workers.Material;

namespace Core.Elements.Windows.Pdf.Data
{
	/// <summary>
	/// Устаревшее название - "PdfMaterial"
	/// </summary>
	[MaterialDataWorker(typeof(PdfMaterialDataWorker))]
	[WindowState(typeof(PdfState))]
	public class PdfMaterialData : FileMaterialData
	{
		[MaterialDataPropertyParse("view"), MaterialDataPropertyUpdate]
		public string View { get; set; }
		
		[MaterialDataPropertyParse("file/pngPaths"), MaterialDataPropertyUpdate]
		public string[] FilePaths { get; set; }
	}
}