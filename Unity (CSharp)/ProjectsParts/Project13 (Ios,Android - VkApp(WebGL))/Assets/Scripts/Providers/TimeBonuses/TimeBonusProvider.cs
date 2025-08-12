using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.Saves;
using Game.Providers.TimeBonuses.Save;
using Game.Providers.TimeBonuses.Settings;
using UnityEngine.Events;
using Zenject;

namespace Game.Providers.TimeBonuses
{
	public class TimeBonusProvider : ITimeBonusProvider
	{
		public UnityEvent OnTimeBonusChangeEvent { get; } = new();

		private TimeBonusSettings _settings;
		private ISaveProvider _saveGame;
		private TimeBonusSave _save;
		private SignalBus _signalBus;
		private List<Base.TimeBonus> _gifts = new();

		public List<Base.TimeBonus> Gifts => _gifts;
		public TimeBonusSave Save => _save;
		public int CountReady => _gifts.FindAll(x => x.GetReady).Count;
		public bool IsLoaded { get; private set; }

		public TimeBonusProvider(SignalBus signalBus, ISaveProvider saveGame, TimeBonusSettings settings)
		{
			_signalBus = signalBus;
			_saveGame = saveGame;
			_settings = settings;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveGame.GetProperty<TimeBonusSave>();

			for (var i = 0; i < _settings.TimeBonuses.Count; i++)
			{
				Base.TimeBonus gift = new();
				var settingsData = _settings.TimeBonuses[i];
				var saveData = _save.Value.Items.Find(x => x.Uuid == settingsData.Uuid);
				if (saveData == null)
				{
					saveData = new();
					saveData.Uuid = settingsData.Uuid;
					saveData.LastGet = DateTime.MinValue;
					_save.Value.Items.Add(saveData);
					_ = _save.Save();
				}

				gift.SetData(settingsData, saveData);
				_ = gift.TimerToReady.OnComplete(EmitChange);
				_gifts.Add(gift);
			}
			EmitChange();
			await UniTask.Yield();
		}

		public void EmitChange() =>
			OnTimeBonusChangeEvent?.Invoke();
	}
}
