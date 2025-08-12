using StringDrop;
using System;

namespace Game.Base {
	[Serializable]
	public class CalculablePlayerResources {
		[StringDropList(typeof(RewardTypes))] public string Type;
		public float Value;

		public string ValueToString => Value.ToString().Replace(",", ".");
	}
}
