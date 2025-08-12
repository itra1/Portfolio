using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Common.Attributes;
using Game.Providers.DailyBonus;
using Game.Providers.Ui.Elements;
using Game.Providers.Ui.Popups.Base;
using Game.Providers.Ui.Popups.Common;
using UnityEngine;
using Zenject;

namespace Game.Providers.Ui.Popups.Elements {
	[PrefabName(PopupsNames.DailyBonus)]
	public class DailyBonusPopup :Popup {
		[SerializeField] private List<DailyBonusView> _items;

		private DailyBonusProvider _provider;
		private DailyBonusHandler _dailyBonusHandler;

		[Inject]
		public void Constructor(DailyBonusProvider provider, DailyBonusHandler dailyBonusHandler) {
			_provider = provider;
			_dailyBonusHandler = dailyBonusHandler;
		}

		protected override void OnEnable() {
			base.OnEnable();
			MakeList();
		}

		private void MakeList() {
			for (var i = 0; i < _items.Count && i < _provider.DailyBonusList.Count; i++) {
				_items[i].SetData(_provider.DailyBonusList[i]);
				_items[i].OnGet = () => {
					Hide().Forget();
				};
			}
		}

		public void CloseButtonTouch() {
			for (var i = 0; i < _items.Count; i++) {
				if (_items[i].IsGetReady)
					_items[i].GetReward();
			}
		}

	}
}
