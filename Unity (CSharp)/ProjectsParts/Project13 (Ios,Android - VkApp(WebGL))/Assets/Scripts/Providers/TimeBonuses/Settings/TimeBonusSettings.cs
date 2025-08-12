using Game.Base;
using StringDrop;
using System.Collections.Generic;
using UnityEngine;
using Uuid;

namespace Game.Providers.TimeBonuses.Settings {
	[System.Serializable]
	public class TimeBonusSettings {
		public List<TimeBonusItemSettings> TimeBonuses;
	}

	[System.Serializable]
	public struct TimeBonusItemSettings {
		[UUID] public string Uuid;
		[Tooltip("Секунды")] public float Period;
		[StringDropList(typeof(RewardTypes))] public string RewardType;
		public float Value;
	}
}
