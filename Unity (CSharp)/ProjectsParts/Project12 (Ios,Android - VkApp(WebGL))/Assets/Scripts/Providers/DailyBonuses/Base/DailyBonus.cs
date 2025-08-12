using UnityEngine;

namespace Game.Scripts.Providers.DailyBonuses.Base
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "DailyBonus", menuName = "Providers/DailyBonus/DailyBonus")]
	public class DailyBonus : ScriptableObject
	{
		[SerializeField] private int _dayIndex;
	}
}
