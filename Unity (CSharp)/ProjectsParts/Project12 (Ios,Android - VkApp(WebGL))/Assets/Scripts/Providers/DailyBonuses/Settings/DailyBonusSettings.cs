using System.Collections.Generic;
using Game.Scripts.Providers.DailyBonuses.Base;
using StringDrop;
using UnityEngine;

namespace Game.Scripts.Providers.DailyBonuses.Settings
{
	[System.Serializable]
	public class DailyBonusSettings
	{
		[SerializeField] private List<BonusItemSettings> _bonusList;

		public List<BonusItemSettings> BonusList => _bonusList;
	}

	[System.Serializable]
	public class BonusItemSettings
	{
		[SerializeField][StringDropList(typeof(BonusType))] private string _type;
		[SerializeField] private double _secondsPeriod;
		[SerializeField] private bool _isAds;

		public string Type => _type;
		public double SecondsPeriod => _secondsPeriod;
		public bool IsAds => _isAds;
	}
}
