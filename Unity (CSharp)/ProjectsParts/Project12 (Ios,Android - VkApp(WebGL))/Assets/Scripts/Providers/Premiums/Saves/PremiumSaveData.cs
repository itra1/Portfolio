using System;
using Game.Scripts.Providers.Saves.Data;

namespace Game.Scripts.Providers.Premiums.Saves
{
	public class PremiumSaveData : SaveProperty<PremiumSave>
	{
		public override PremiumSave DefaultValue => new();
	}

	[System.Serializable]
	public class PremiumSave
	{
		public bool ActivePremium;
		public DateTime ActiveteDate;
		public DateTime NextReward;
	}
}
