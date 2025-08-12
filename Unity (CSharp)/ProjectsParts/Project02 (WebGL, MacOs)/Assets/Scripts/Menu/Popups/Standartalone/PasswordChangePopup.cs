using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Managers;
using it.UI.Elements;

namespace it.Popups
{
	public class PasswordChangePopup : PopupBase
	{
		[SerializeField] private PasswordChangeForm _passwordForm;
		[SerializeField] private PasswordChangeSuccessfully _successForm;

		protected override void EnableInit()
		{
			_passwordForm.gameObject.SetActive(true);
			_successForm.gameObject.SetActive(false);
			_passwordForm.OnConfirm = () =>
			{

				_passwordForm.gameObject.SetActive(false);
				_successForm.gameObject.SetActive(true);
			};

			_successForm.OnConfirm = () =>
			{
				Hide();
			};

		}

	}
}