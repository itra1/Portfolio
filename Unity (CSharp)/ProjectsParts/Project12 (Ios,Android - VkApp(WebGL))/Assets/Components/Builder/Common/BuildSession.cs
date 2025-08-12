using Builder.Platforms;
using UnityEditor;
using UnityEngine.UIElements;

namespace Builder.Common
{
	public class BuildSession
	{
		public string Version { get; set; }
		public string Platform { get; set; }
		public VisualElement RootElement { get; set; }
		public VisualElement BodyElement { get; set; }
		public Settings Settings { get; set; }
		public BuilderWindow Window { get; set; }
		public Archivate ArchivateHelper { get; set; }
		public PlatformBuilder Builder { get; set; }
		public BuildOptions BuildOptions { get; set; } = BuildOptions.None;
	}
}
