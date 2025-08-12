using System.Collections.Generic;

namespace Scripts.GameItems.Platforms {
	[System.Serializable]
	public class PlatformElementsGroup {
		public string Uuid;
		public bool ExistsFamage;
		public List<PlatformFormationItem> Platforms;
	}
}
