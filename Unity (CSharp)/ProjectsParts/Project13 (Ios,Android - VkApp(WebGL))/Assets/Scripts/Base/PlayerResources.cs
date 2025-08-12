using StringDrop;
using System;
using UnityEngine;

namespace Game.Base {
	[Serializable]
	public class PlayerResources {
		[SerializeField, StringDropList(typeof(RewardTypes))]
		public string Type;
		public float Value;

		public string ValueAsString => Math.Round(Value, 1).ToString().Replace(",", ".");
	}
}