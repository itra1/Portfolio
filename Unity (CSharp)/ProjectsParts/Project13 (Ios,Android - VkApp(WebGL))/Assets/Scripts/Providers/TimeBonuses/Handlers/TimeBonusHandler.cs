using Game.Providers.Profile.Signals;
using Game.Providers.TimeBonuses.Base;
using UnityEngine;
using Zenject;

namespace Game.Providers.TimeBonuses.Handlers
{
	public class TimeBonusHandler
	{
		private ITimeBonusProvider _timeBonusProvider;
		private SignalBus _signalBus;

		public TimeBonusHandler(SignalBus signalBus, ITimeBonusProvider timeBonusProvider)
		{
			_timeBonusProvider = timeBonusProvider;
			_signalBus = signalBus;
		}

		public void Confirm(ITimeBonus gift, RectTransform point)
		{
			GetGift(gift, point);
		}
		private void GetGift(ITimeBonus gift, RectTransform point)
		{
			_signalBus.Fire(new ResourceAddSignal(gift.RewardType, gift.Value, point));
			gift.Get();
			_timeBonusProvider.Save.Save();
			_timeBonusProvider.EmitChange();
		}
	}
}
