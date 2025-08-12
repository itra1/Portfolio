using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Providers.DailyBonuses.Items;
using Game.Scripts.Providers.DailyBonuses.Save;
using Game.Scripts.Providers.DailyBonuses.Settings;
using Game.Scripts.Providers.Saves;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Providers.DailyBonuses
{
	public class DailyBonusProvider : IDailyBonusProvider
	{
		public UnityEvent<IBonus> OnBonusChange = new();

		private readonly DailyBonusSettings _settings;
		private ISaveProvider _saveProvider;
		private DiContainer _diContainer;
		private DailyBonusSave _saveData;
		private List<IBonus> _bonusList = new();

		public List<IBonus> BonusList => _bonusList;

		public DailyBonusProvider(DailyBonusSettings settings, ISaveProvider saveProvider, DiContainer diContainer)
		{
			_settings = settings;
			_saveProvider = saveProvider;
			_diContainer = diContainer;
		}

		public async UniTask StartAppLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_saveData = _saveProvider.GetProperty<DailyBonusSaveData>().Value;
			StartCreateBonuses();
			await UniTask.Yield();
		}

		private void StartCreateBonuses()
		{
			foreach (var item in _settings.BonusList)
			{
				var instance = new Bonus();
				_diContainer.Inject(instance);
				instance.SetSettings(item);

				var saveData = _saveData.SaveItems.Find(x => x.Type == instance.Type);

				if (saveData != null)
				{
					instance.SetSaveData(saveData);
				}
				else
				{
					_saveData.SaveItems.Add(instance.GetSaveData());
				}

				instance.OnChangeState.AddListener(OnBonusChangeListener);

				_bonusList.Add(instance);
			}
		}

		private void OnBonusChangeListener(IBonus bonus)
		{
			OnBonusChange?.Invoke(bonus);
		}

		public void SelectBonus(string type)
		{
			var bonus = _bonusList.Find(x => x.Type == type);

			if (bonus == null)
				return;

			if (!bonus.RewardReady)
				return;

			bonus.Reward();
		}
	}
}
