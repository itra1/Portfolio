using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

namespace it.Popups
{
	public sealed class WelcomeBonusPopup : PopupBase
	{
		[SerializeField] private TextMeshProUGUI _titleLabel;
		[SerializeField] private TextMeshProUGUI[] _values;
		[SerializeField] private GameObject[] _infoBlocks;
		[SerializeField] private RectTransform _progress;

		protected override void EnableInit()
		{
			base.EnableInit();
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WelcomeBonusUpdate, WelcomeBonusUpdateEvent);
			ConfirmData();
		}


		private void WelcomeBonusUpdateEvent(com.ootii.Messages.IMessage hendle)
		{
			ConfirmData();
		}

		private void VisibleInfoBlock(int index)
		{
			for (int i = 0; i < _infoBlocks.Length; i++)
			{
				_infoBlocks[i].gameObject.SetActive(i == index);
			}
		}

		private void FillBonusValues(double fullValue)
		{
			for (int i = 0; i <= _infoBlocks.Length; i++)
			{
				_values[i].text = $"{StringConstants.CURRENCY_SYMBOL}{fullValue / 5 * (i + 1)}";
			}
		}
		private void FillProgress(int level)
		{
			switch (level){
				case 0:
					_progress.anchoredPosition = new Vector2(-333, 0);
					break;
				case 1:
					_progress.anchoredPosition = new Vector2(-262.7f, 0);
					break;
				case 2:
					_progress.anchoredPosition = new Vector2(-197.8f, 0);
					break;
				case 3:
					_progress.anchoredPosition = new Vector2(0 - 134.4f, 0);
					break;
				case 4:
					_progress.anchoredPosition = new Vector2(-70.5f, 0);
					break;
				case 5:
					_progress.anchoredPosition = new Vector2(0, 0);
					break;
			}
		}

		private void ConfirmData()
		{

			WelcomeBonusData wb = UserController.Instance.WelcomeBonus;

			if (wb == null || wb.last_bonus == null)
			{
				_titleLabel.text = $"<sprite index=0> Welcome <color=#C68C43>200%";
				VisibleInfoBlock(0);
				FillBonusValues(100);
				FillProgress(0);
				return;
			}
			if ((bool)wb.last_bonus.finished && wb.next_level != null)
			{
				_titleLabel.text = $"<sprite index={wb.last_bonus.level-1}> Welcome <color=#C68C43>{wb.last_bonus.multiplier*100}%";
				VisibleInfoBlock(wb.next_level.level - 1);
				FillBonusValues(wb.last_bonus.bonus_amount);
				FillProgress((int)wb.last_bonus.current_stage);
				return;
			}

			FillProgress((int)wb.last_bonus.current_stage);
			_titleLabel.text = $"<sprite index={wb.last_bonus.level-1}> Welcome <color=#C68C43>{wb.last_bonus.multiplier * 100}%";
			VisibleInfoBlock(wb.last_bonus.level - 1);
			FillBonusValues(wb.last_bonus.bonus_amount);
		}

		public void OkButton()
		{
			Hide();
		}

		public override void Hide()
		{
			base.Hide();
		}
		public void InfoButton()
		{
			Hide();
			it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		}
	}

}
