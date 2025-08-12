using Game.Base;
using StringDrop;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Game.Settings {
	[System.Serializable]
	public class ResourcesIconsSettings {
		public List<ResourceType> Resource = new();
	}

	[System.Serializable]
	public struct ResourceType {
		[StringDropList(typeof(RewardTypes))]
		public string Name;
		public string VisibleName;
		public Sprite IconeBig;
		public Sprite Icone;
		public Sprite IconeMini;
		public Sprite IconeGroup;
	}
}
