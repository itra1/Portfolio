using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Providers.DailyBonus.Elements;
using Game.Providers.DailyBonus.Save;
using Game.Providers.DailyBonus.Settings;
using Game.Providers.Saves;

namespace Game.Providers.DailyBonus
{
	public class DailyBonusProvider : IDailyBonusProvider
	{
		private DailyBonusSettings _settings;
		private DailyBonusSave _save;
		private ISaveProvider _saveProvider;
		public DailyBonusSave Save => _save;
		private List<DailyBonusItem> _dailyBonusList = new();

		public int NextIndex => Save.Value.LastDayGet + 1;
		public bool NextReady => (DateTime.Now - Save.Value.LastDateGet).Days >= 1
		&& _settings.BonusList.Count > NextIndex;

		public List<DailyBonusItem> DailyBonusList { get => _dailyBonusList; set => _dailyBonusList = value; }
		public bool IsLoaded { get; private set; }

		public DailyBonusProvider(DailyBonusSettings settings, ISaveProvider saveProvider)
		{
			_settings = settings;
			_saveProvider = saveProvider;
		}

		public async UniTask FirstLoad(IProgress<float> OnProgress, CancellationToken cancellationToken)
		{
			_save = _saveProvider.GetProperty<DailyBonusSave>();
			MakeList();
			await UniTask.NextFrame();
		}

		private void MakeList()
		{
			for (var i = 0; i < _settings.BonusList.Count; i++)
			{
				var itm = _settings.BonusList[i];
				var item = new DailyBonusItem(i, this, itm);
				_dailyBonusList.Add(item);
			}
		}
	}
}
