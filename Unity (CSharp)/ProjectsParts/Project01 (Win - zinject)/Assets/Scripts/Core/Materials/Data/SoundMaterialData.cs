using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Materials.Data
{
	[MaterialDataLoader("/sounds")]
	public class SoundMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("file"), MaterialDataPropertyUpdate]
		public FileInfo File { get; set; }

		public SoundMaterialData()
		{
			Model = MaterialModel.Sounds;
			Category = MaterialCategory.Sounds;
		}
	}
	
	public class FileInfo
	{
		[MaterialDataPropertyParse("fieldname")]
		public string FieldName { get; set; }

		[MaterialDataPropertyParse("originalname")]
		public string OriginalName { get; set; }

		[MaterialDataPropertyParse("encoding")] 
		public string Encoding { get; set; }
		
		[MaterialDataPropertyParse("mimetype")] 
		public string Mimetype { get; set; }

		[MaterialDataPropertyParse("destination")]
		public string Destination { get; set; }

		[MaterialDataPropertyParse("filename")] 
		public string Filename { get; set; }
		
		[MaterialDataPropertyParse("path")] 
		public string Path { get; set; }
		
		[MaterialDataPropertyParse("size")] 
		public long Size { get; set; }
		
		[MaterialDataPropertyParse("url")] 
		public string Url { get; set; }
		
		[MaterialDataPropertyParse("src")] 
		public string Src { get; set; }
		
		[MaterialDataPropertyParse("title")] 
		public string Title { get; set; }
	}
}