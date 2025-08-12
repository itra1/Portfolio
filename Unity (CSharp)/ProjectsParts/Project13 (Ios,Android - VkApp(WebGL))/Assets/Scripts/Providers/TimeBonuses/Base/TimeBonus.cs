using System;
using Game.Providers.TimeBonuses.Save;
using Game.Providers.TimeBonuses.Settings;
using Game.Providers.Timers;
using Game.Providers.Timers.Base;
using Game.Providers.Timers.Common;
using UnityEngine.Events;

namespace Game.Providers.TimeBonuses.Base {
	[System.Serializable]
	public class TimeBonus :ITimeBonus {

		private TimeBonusItemSettings _settings;
		private TimeBonusSaveItem _saveItem;

		public UnityEvent OnChange { get; } = new();

		public string Uuid => _settings.Uuid;
		public string RewardType => _settings.RewardType;
		public float Value => _settings.Value;
		public bool GetReady => (DateTime.Now - _saveItem.LastGet).TotalSeconds > _settings.Period;
		public DateTime NextGet => _saveItem.LastGet.AddSeconds(_settings.Period);
		public ITimer TimerToReady { get; private set; }
		public TimeBonusSaveItem Save => _saveItem;

		public void SetData(TimeBonusItemSettings settings, TimeBonusSaveItem saveItem) {
			_settings = settings;
			_saveItem = saveItem;
			TimerToReady ??= TimersProvider.Create(TimerType.RealtimeDesc).AutoRemove(false);
			StartTimerIfNeed();
		}

		public void Get() {
			_saveItem.LastGet = DateTime.Now;
			StartTimerIfNeed();
			OnChange?.Invoke();
		}

		public void StartTimerIfNeed() {
			if (!GetReady) {
				StartTimer();
			}
		}

		public void StartTimer() {
			TimerToReady.End(NextGet)
			.Start();
		}
	}
}
