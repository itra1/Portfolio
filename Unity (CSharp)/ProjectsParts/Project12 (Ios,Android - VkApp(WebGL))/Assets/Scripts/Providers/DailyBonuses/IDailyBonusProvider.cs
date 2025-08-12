using System.Collections.Generic;
using Game.Scripts.Providers.DailyBonuses.Items;
using Game.Scripts.App;

namespace Game.Scripts.Providers.DailyBonuses
{
	public interface IDailyBonusProvider : IApplicationLoaderItem
	{
		List<IBonus> BonusList { get; }

		void SelectBonus(string type);
	}
}