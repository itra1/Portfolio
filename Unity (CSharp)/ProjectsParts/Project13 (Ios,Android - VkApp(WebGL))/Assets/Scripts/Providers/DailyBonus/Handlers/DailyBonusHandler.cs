using System;
using Cysharp.Threading.Tasks;
using Game.Providers.DailyBonus.Elements;
using Game.Providers.DailyBonus.Settings;
using Game.Providers.DailyBonus.Signals;
using Game.Providers.Profile.Signals;
using Game.Providers.Ui.Popups;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using Zenject;

namespace Game.Providers.DailyBonus {
	public class DailyBonusHandler {
		private DailyBonusProvider _provider;
		private IPopupsProvider _popupsProvider;
		private DailyBonusSettings _settings;
		private SignalBus _signalBus;

		public bool ExistsDailyBonus => _provider.NextReady;
		public DailyBonusHandler(SignalBus signalBus, DailyBonusProvider provider, IPopupsProvider popupProvider,
		DailyBonusSettings settings) {
			_signalBus = signalBus;
			_provider = provider;
			_popupsProvider = popupProvider;
			_settings = settings;
		}

		public async UniTask Open() {
			var dailyBonusPopup = _popupsProvider.GetPopup(PopupsNames.DailyBonus);
			await dailyBonusPopup.Show();
		}

		internal void GetBonus(DailyBonusItem dailyBonus, RectTransform point) {

			for (var i = 0; i < dailyBonus.Setting.Rewards.Length; i++) {
				_signalBus.Fire(new ResourceAddSignal(dailyBonus.Setting.Rewards[i].Reward, dailyBonus.Setting.Rewards[i].Value, point));
			}

			_provider.Save.Value.LastDayGet = dailyBonus.Index;
			_provider.Save.Value.LastDateGet = DateTime.Now;
			_provider.Save.Save();
			_signalBus.Fire<DailyBonusChangeSignal>();
		}
	}
}
