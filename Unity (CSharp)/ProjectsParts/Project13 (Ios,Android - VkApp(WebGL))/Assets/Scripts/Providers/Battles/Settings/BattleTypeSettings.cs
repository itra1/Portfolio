using Game.Providers.Battles.Base;
using StringDrop;
using UnityEngine;

namespace Game.Providers.Battles.Settings
{
	public class BattleTypeSettings : ScriptableObject
	{
		[StringDropList(typeof(BattleType))]
		[SerializeField] private string _battleType;

		public string BattleType => _battleType;
	}
}
