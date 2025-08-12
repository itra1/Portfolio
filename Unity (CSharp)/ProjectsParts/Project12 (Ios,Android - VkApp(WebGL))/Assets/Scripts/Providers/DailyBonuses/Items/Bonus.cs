using System;
using Game.Scripts.Providers.DailyBonuses.Save;
using Game.Scripts.Providers.DailyBonuses.Settings;
using Game.Scripts.Providers.Timers;
using Game.Scripts.Providers.Timers.Base;
using Game.Scripts.Providers.Timers.Common;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Providers.DailyBonuses.Items
{
	public class Bonus : IBonus
	{
		public UnityEvent<IBonus> OnChangeState { get; set; } = new();

		private string _type;
		private ITimer _timer;
		private double _secondsDelay;
		private bool _isAds;
		private DailyBonusItemSave _saveData;
		private ITimersProvider _timersProvider;

		public string Type => _type;
		public ITimer Timer => _timer;
		public bool RewardReady => _saveData.TimeComplete <= DateTime.Now;

		[Inject]
		private void Constructor(ITimersProvider timersProvider)
		{
			_timersProvider = timersProvider;
		}

		public void SetSettings(BonusItemSettings settings)
		{
			_type = settings.Type;
			_secondsDelay = settings.SecondsPeriod;
			_isAds = settings.IsAds;
		}

		public void SetSaveData(DailyBonusItemSave data)
		{
			_saveData = data;
			StartTimerDelay();
		}

		public DailyBonusItemSave GetSaveData()
		{
			if (_saveData == null)
			{
				_saveData = new DailyBonusItemSave();
				_saveData.Type = _type;
				_saveData.TimeComplete = DateTime.Now.AddSeconds(_secondsDelay);
			}
			StartTimerDelay();
			return _saveData;
		}

		public void Reward()
		{
			if (!RewardReady)
				return;

			SetNewTimeDelay();
			StartTimerDelay();
		}

		private void SetNewTimeDelay()
		{
			_saveData.TimeComplete = DateTime.Now.AddSeconds(_secondsDelay);
		}

		private void StartTimerDelay()
		{
			if (_timer != null && _timer.IsActive)
			{
				_ = _timer.Stop();
			}
			_timer = _timersProvider
			.Create(TimerType.RealtimeDesc)
			.End((_saveData.TimeComplete - DateTime.Now).TotalSeconds)
			.OnComplete(() =>
			{
				OnChangeState?.Invoke(this);
			})
			.Start();
			OnChangeState?.Invoke(this);
		}
	}
}
