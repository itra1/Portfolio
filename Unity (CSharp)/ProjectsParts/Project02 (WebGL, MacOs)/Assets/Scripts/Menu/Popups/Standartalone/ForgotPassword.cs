using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace it.Popups
{
	public class ForgotPassword : PopupBase
	{
		[SerializeField] private GameObject _forgotBlock;
		[SerializeField] private GameObject _completeBlock;
		[SerializeField] private TMP_InputField _inputField;
		[SerializeField] private Button _closeButton;

		protected override void EnableInit()
		{
			base.EnableInit();
			_inputField.ActivateInputField();
			_forgotBlock.gameObject.SetActive(true);
			_completeBlock.gameObject.SetActive(false);
		}

		public void RecoveryButton()
		{
			//todo реализовать вызов функции восстановления пароля
			_forgotBlock.gameObject.SetActive(false);
			_completeBlock.gameObject.SetActive(true);
			_closeButton.Select();
		}

		public void LoginButton()
		{
			Hide();
			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
		}
	}
}