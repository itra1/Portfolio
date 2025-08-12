using System.Collections.Generic;
using Game.Base.AppLaoder;
using Game.Providers.TimeBonuses.Base;
using Game.Providers.TimeBonuses.Save;
using UnityEngine.Events;

namespace Game.Providers.TimeBonuses
{
	public interface ITimeBonusProvider : IAppLoaderElement
	{
		public UnityEvent OnTimeBonusChangeEvent { get; }
		TimeBonusSave Save { get; }
		int CountReady { get; }
		List<TimeBonus> Gifts { get; }

		void EmitChange();
	}
}