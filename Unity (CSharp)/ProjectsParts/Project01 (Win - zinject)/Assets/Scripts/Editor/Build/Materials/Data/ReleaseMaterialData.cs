using System;
using System.Collections.Generic;
using Core.Materials.Attributes;

namespace Editor.Build.Materials.Data
{
	/// <summary>
	/// Устаревшее навзание - "RelelaseMaterial"
	/// </summary>
	public class ReleaseMaterialData
	{
		[MaterialDataPropertyParse("id")]
		public ulong Id { get; set; }
		
		[MaterialDataPropertyParse("type")]
		public string Type { get; set; }
		
		[MaterialDataPropertyParse("description"), MaterialDataPropertyUpdate]
		public string Description { get; set; }
		
		[MaterialDataPropertyParse("version"), MaterialDataPropertyUpdate]
		public string Version { get; set; }
		
		[MaterialDataPropertyParse("checksum"), MaterialDataPropertyUpdate]
		public string Checksum { get; set; }

		[MaterialDataPropertyParse("tagsIds"), MaterialDataPropertyUpdate]
		public List<ulong> TagIds { get; set; } = new ();
		
		[MaterialDataPropertyParse("createdAt"), MaterialDataPropertyUpdate]
		public string CreatedAt { get; set; }
		
		public DateTime CreateTime => DateTime.Parse(CreatedAt);

#if UNITY_EDITOR
		public int SelectedTagsMask { get; set; }
		public int SelectTags;
		public bool IsLocal { get; set; } = false;
		public bool IsServer { get; set; } = false;
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public bool IsChangeDescription { get; set; }
#endif
	}
}