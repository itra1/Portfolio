using System.Collections.Generic;
using Engine.Scripts.Base;
using Game.Scripts.Providers.DailyBonuses;
using Game.Scripts.Providers.DailyBonuses.Base;
using Game.Scripts.Providers.DailyBonuses.Items;
using Game.Scripts.Providers.Timers.Helpers;
using Game.Scripts.Providers.Timers.Ui;
using Game.Scripts.UI.Presenters.Interfaces;
using StringDrop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Components
{
	public class DailyBonusHomePanel : MonoBehaviour, IInjection, IUiVisibleHandler
	{
		[SerializeField] private List<ButtonBonus> _bonusButtons;

		private IDailyBonusProvider _dailyBonusProvider;

		[Inject]
		public void Constructor(IDailyBonusProvider dailyBonusProvider)
		{
			_dailyBonusProvider = dailyBonusProvider;
			foreach (var item in _bonusButtons)
			{
				item.Button.onClick.AddListener(() => ButtonSelfTouch(item));
			}
		}

		public void Show()
		{
			for (int i = 0; i < _dailyBonusProvider.BonusList.Count; i++)
			{
				_dailyBonusProvider.BonusList[i].OnChangeState.AddListener(OnBonusChangeListener);
			}
			SetVisible();
		}

		public void Hide()
		{
			for (int i = 0; i < _dailyBonusProvider.BonusList.Count; i++)
			{
				_dailyBonusProvider.BonusList[i].OnChangeState.RemoveListener(OnBonusChangeListener);
			}
		}

		private void SetVisible()
		{
			for (int i = 0; i < _bonusButtons.Count; i++)
			{
				var bonusButton = _bonusButtons[i];
				var bonus = _dailyBonusProvider.BonusList.Find(x => x.Type == bonusButton.Type);

				if (bonus == null)
					continue;


				VisibleElement(bonusButton, bonus);
			}
		}

		private void OnBonusChangeListener(IBonus bonus)
		{
			var bonusButton = _bonusButtons.Find(x => x.Type == bonus.Type);

			if (bonusButton == null)
				return;

			VisibleElement(bonusButton, bonus);
		}

		private void VisibleElement(ButtonBonus bonusButton, IBonus bonus)
		{
			if (bonus.RewardReady)
			{
				bonusButton.DisablePanel.SetActive(false);
				if (bonusButton.Icone != null)
					bonusButton.Icone.SetActive(true);
			}
			else
			{
				bonusButton.DisablePanel.SetActive(true);
				if (bonusButton.Icone != null)
					bonusButton.Icone.SetActive(false);
				_ = bonus.Timer.OnTick((sec) =>
				{
					TimerBlockVisible.VisibleBlock(bonusButton.TimerBlock, sec);
				});
				TimerBlockVisible.VisibleBlock(bonusButton.TimerBlock, bonus.Timer.CurrentValueSeconds);
			}
		}

		private void ButtonSelfTouch(ButtonBonus item)
		{
			_dailyBonusProvider.SelectBonus(item.Type);
		}

		[System.Serializable]
		public class ButtonBonus
		{
			[StringDropList(typeof(BonusType))]
			public string Type;
			public Button Button;
			public TimerPresenterBlock TimerBlock;
			public GameObject DisablePanel;
			public GameObject Icone;
		}
	}
}
