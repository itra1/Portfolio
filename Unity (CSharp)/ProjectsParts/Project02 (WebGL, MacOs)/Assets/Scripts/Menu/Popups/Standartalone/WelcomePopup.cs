using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace it.Popups
{
	public class WelcomePopup : PopupBase
	{
		[SerializeField] private TextMeshProUGUI _welcomeLabel;
		[SerializeField] private TextMeshProUGUI _goodLuckLabel;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _playButton;

		protected override void EnableInit()
		{
			base.EnableInit();
			if (_playButton != null)
				_playButton.Select();
			CashierVisibleChange();
		}

		protected override void Localize()
		{
			base.Localize();
			_welcomeLabel.text = string.Format(I2.Loc.LocalizationManager.GetTranslation("popup.welcome.hello"), UserController.User.nickname);
			_goodLuckLabel.text = string.Format(I2.Loc.LocalizationManager.GetTranslation("popup.welcome.goodLuck"), UserController.User.nickname);
		}

		public void PlayButton()
		{
			Hide();
		}

		public void CashierButton()
		{
			Hide();
			it.Main.PopupController.Instance.ShowPopup(PopupType.Cashier);
		}
		private void CashierVisibleChange()
		{
#if UNITY_IOS
			_playButton?.gameObject.SetActive(AppConfig.ActiveCashier);
#endif
		}

		private void CashierVisibleconfirm(bool value = false)
		{
			AppConfig.ActiveCashier = value;
			com.ootii.Messages.MessageDispatcher.SendMessage(EventsConstants.CashierVisibleChange);
		}
	}
}