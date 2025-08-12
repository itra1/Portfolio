using Game.Scripts.Providers.Timers.Common;
using UnityEngine.Events;

namespace Game.Scripts.Providers.DailyBonuses.Items
{
	public interface IBonus
	{
		string Type { get; }
		ITimer Timer { get; }
		bool RewardReady { get; }
		UnityEvent<IBonus> OnChangeState { get; set; }

		void Reward();
	}
}