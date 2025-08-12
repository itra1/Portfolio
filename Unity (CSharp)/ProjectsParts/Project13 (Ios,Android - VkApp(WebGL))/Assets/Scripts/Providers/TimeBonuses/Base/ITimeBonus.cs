using System;
using Game.Base.Interfaces;
using Game.Providers.TimeBonuses.Save;
using Game.Providers.Timers.Common;
using UnityEngine.Events;

namespace Game.Providers.TimeBonuses.Base {
	public interface ITimeBonus :IUuid {
		string RewardType { get; }
		float Value { get; }
		bool GetReady { get; }
		TimeBonusSaveItem Save { get; }
		DateTime NextGet { get; }
		ITimer TimerToReady { get; }
		UnityEvent OnChange { get; }
		void Get();
	}
}
