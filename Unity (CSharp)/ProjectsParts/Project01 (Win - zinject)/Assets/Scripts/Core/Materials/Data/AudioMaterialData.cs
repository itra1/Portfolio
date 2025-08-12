using System.Text;
using Core.Materials.Attributes;
using Core.Materials.Consts;

namespace Core.Materials.Data
{
	/// <summary>
	/// Устаревшее название - "AudioMaterialData"
	/// </summary>
	[MaterialDataLoader("/audio")]
	public class AudioMaterialData : MaterialData
	{
		[MaterialDataPropertyParse("file"), MaterialDataPropertyUpdate]
		public AudioFileItemData File { get; set; }
		
		[MaterialDataPropertyParse("createdAt"), MaterialDataPropertyUpdate]
		public string CreatedAt { get; set; }

		public AudioMaterialData()
		{
			Model = MaterialModel.Audio;
			Category = MaterialCategory.Audio;
		}
	}
	
	public class AudioFileItemData
	{
		[MaterialDataPropertyParse("fieldname"), MaterialDataPropertyUpdate]
		public string FieldName { get; set; }
		
		[MaterialDataPropertyParse("originalname"), MaterialDataPropertyUpdate]
		public string OriginalName { get; set; }
		
		[MaterialDataPropertyParse("encoding"), MaterialDataPropertyUpdate]
		public string Encoding { get; set; }
		
		[MaterialDataPropertyParse("mimetype"), MaterialDataPropertyUpdate]
		public string Mimetype { get; set; }
		
		[MaterialDataPropertyParse("destination"), MaterialDataPropertyUpdate]
		public string Destination { get; set; }
		
		[MaterialDataPropertyParse("path"), MaterialDataPropertyUpdate]
		public string Path { get; set; }
		
		[MaterialDataPropertyParse("size"), MaterialDataPropertyUpdate]
		public int Size { get; set; }
		
		[MaterialDataPropertyParse("duration"), MaterialDataPropertyUpdate]
		public float Duration { get; set; }
		
		[MaterialDataPropertyParse("url"), MaterialDataPropertyUpdate]
		public string Url { get; set; }
		
		[MaterialDataPropertyParse("src"), MaterialDataPropertyUpdate]
		public string Src { get; set; }
		
		[MaterialDataPropertyParse("title"), MaterialDataPropertyUpdate]
		public string Title { get; set; }

		public override string ToString()
		{
			var buffer = new StringBuilder();
			
			buffer.Append('{');
			
			if (!string.IsNullOrEmpty(OriginalName))
				buffer.Append($"name: {OriginalName}, ");
			
			if (!string.IsNullOrEmpty(Url))
				buffer.Append($"url: {Url}");
			
			buffer.Append('}');
			
			return buffer.ToString();
		}
	}
}