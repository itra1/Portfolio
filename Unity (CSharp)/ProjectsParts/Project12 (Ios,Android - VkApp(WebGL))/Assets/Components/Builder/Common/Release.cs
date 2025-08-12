using System;

namespace Builder.Common
{
	public class Release
	{
		public string Version { get; set; }
		public DateTime CreateTime { get; set; }
		public string Description { get; set; }
		public string FileName { get; set; }
		public string Platform { get; set; }
		public string Uploaded { get; set; }
		public bool InServer { get; set; }
	}
}
