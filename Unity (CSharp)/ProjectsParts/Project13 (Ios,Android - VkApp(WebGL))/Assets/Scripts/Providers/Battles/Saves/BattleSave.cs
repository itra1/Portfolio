using System.Collections.Generic;
using Game.Providers.Battles.Settings;
using Game.Providers.Saves.Data;

namespace Game.Providers.Battles.Saves
{
	public class BattleSave : SaveProperty<BattleSaveData>
	{
		public override BattleSaveData DefaultValue => new();
	}
	public class BattleSaveData
	{
		public List<BattleResult> TutorialResult = new();
	}
}
