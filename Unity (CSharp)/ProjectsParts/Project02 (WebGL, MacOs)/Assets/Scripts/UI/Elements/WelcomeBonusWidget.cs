using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
 

namespace it.UI.Elements
{
	public class WelcomeBonusWidget : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _bonusLabel;
		[SerializeField] private TextMeshProUGUI _timeLabel;
		[SerializeField] private TextMeshProUGUI _stageLabel;
		[SerializeField] private TextMeshProUGUI _valueLabel;
		[SerializeField] private RectTransform _progressBlock;
		[SerializeField] private RectTransform _stageProgress;

		private void Start()
		{
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WelcomeBonusUpdate, WelcomeBonusUpdateEvent);
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.LocalizationChange, WelcomeBonusUpdateEvent);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WelcomeBonusUpdate, WelcomeBonusUpdateEvent);
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.LocalizationChange, WelcomeBonusUpdateEvent);
		}

		private void OnEnable()
		{
			ConfirmData();
		}

		private void OnDisable()
		{
		}

		public void ExpandButtonTouch()
		{
			it.Main.SinglePageController.Instance.Show(SinglePagesType.WelcomeBonus);
		}

		private void WelcomeBonusUpdateEvent(com.ootii.Messages.IMessage hendle)
		{
			ConfirmData();
		}

		private void ConfirmData()
		{

			WelcomeBonusData wb = UserController.Instance.WelcomeBonus;

			if (wb == null || wb.last_bonus == null)

			{
				if (_stageLabel != null)
					_stageLabel.gameObject.SetActive(false);
				_progressBlock.gameObject.SetActive(false);
				if (_valueLabel != null)
					_valueLabel.gameObject.SetActive(false);
				_bonusLabel.gameObject.SetActive(true);
				_bonusLabel.text = string.Format("main.user.welcomeBonus.depositBonus".Localized(), "200%");
				_timeLabel.text = "main.user.welcomeBonus.anytime".Localized().Replace("<sprite index=0>", "");

				return;
			}
			//if ((bool)wb.last_bonus.finished && wb.next_level != null)
			//{
			//	if (_stageLabel != null)
			//		_stageLabel.gameObject.SetActive(false);
			//	_progressBlock.gameObject.SetActive(false);
			//	if (_valueLabel != null)
			//		_valueLabel.gameObject.SetActive(false);
			//	_bonusLabel.gameObject.SetActive(true);
			//	_bonusLabel.text = string.Format("main.user.welcomeBonus.depositBonus".Localized(), (wb.next_level.multiplier * 100));
			//	_timeLabel.text = string.Format("main.user.welcomeBonus.days".Localized(), ((wb.last_bonus.ExpiredDate - System.DateTime.Now).Days)).Replace("<sprite index=0>", "");

			//	return;
			//}
			if ((bool)wb.last_bonus.finished /*&& wb.next_level == null*/)
			{
				if (_stageLabel != null)
					_stageLabel.gameObject.SetActive(false);
				_progressBlock.gameObject.SetActive(false);
				if (_valueLabel != null)
					_valueLabel.gameObject.SetActive(false);
				_bonusLabel.gameObject.SetActive(true);
				_bonusLabel.text = string.Format("main.user.welcomeBonus.contactUs".Localized());
				//_bonusLabel.text = string.Format("main.user.welcomeBonus.depositBonus".Localized(), (wb.next_level.multiplier * 100));
				_timeLabel.text = "main.user.welcomeBonus.anytime".Localized().Replace("<sprite index=0>", "");
				//_timeLabel.text = string.Format("main.user.welcomeBonus.days".Localized(), ((wb.last_bonus.ExpiredDate - System.DateTime.Now).Days)).Replace("<sprite index=0>", "");

				return;
			}
			// <sprite index=0>Anytime

			if (_stageLabel != null)
				_stageLabel.gameObject.SetActive(true);
			_progressBlock.gameObject.SetActive(true);
			if (_valueLabel != null)
				_valueLabel.gameObject.SetActive(false);

#if UNITY_ANDROID || UNITY_WEBGL || UNITY_IOS
			_bonusLabel.gameObject.SetActive(true);
#endif
#if UNITY_STANDALONE
			_bonusLabel.gameObject.SetActive(false);
#endif
			if (_stageLabel != null)
				_stageLabel.text = "main.user.welcomeBonus.stage".Localized() + wb.last_bonus.current_stage;
			_stageProgress.localScale = new Vector3(Mathf.Min(1, (float)wb.last_bonus.current_stage / (float)wb.last_bonus.total_stages), 1, 1);
			if (wb.last_bonus != null)
			{
				if (_valueLabel != null)
					_valueLabel.gameObject.SetActive(true);
				_bonusLabel.text = string.Format("main.user.welcomeBonus.depositBonus".Localized(), (wb.last_bonus.multiplier * 100));
				if (_valueLabel != null)
					_valueLabel.text = $"{wb.last_bonus.current_stage * 20}%";
			}
			_timeLabel.text = string.Format("main.user.welcomeBonus.days".Localized(), ((wb.last_bonus.ExpiredDate - System.DateTime.Now).Days)).Replace("<sprite index=0>", "");
		}

	}
}