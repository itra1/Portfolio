using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Managers;
using it.UI.Elements;
using it.Main;

namespace it.Popups
{
	public class RegistrationPopup : PopupBase
	{

		[SerializeField] private RegistrationForm _registrationFormPanel;
		//[SerializeField] private RegistrationSuccess _registrationSuccesPanel;
		[SerializeField] private RegistrationConfirm _confirmFormPanel;
		[SerializeField] private RegistrationSuccess registrationSuccess;

		private string _email;
		private bool _fromLogin;
		private System.DateTime _registrDatetime;

		protected override void EnableInit()
		{
			base.EnableInit();
			_registrationFormPanel.gameObject.SetActive(true);
			//_registrationSuccesPanel.gameObject.SetActive(false);
			_confirmFormPanel.gameObject.SetActive(false);

			_registrationFormPanel.OnRegistered = (email) =>
			{
				_registrationFormPanel.gameObject.SetActive(false);
				//_registrationSuccesPanel.gameObject.SetActive(true);
				_email = email;
				_registrDatetime = System.DateTime.Now.AddSeconds(30);
				_confirmFormPanel.gameObject.SetActive(true);
				_confirmFormPanel.SetEmail(_email);
				_confirmFormPanel.StartTimer(_registrDatetime);
			};

			_confirmFormPanel.OnRecovery = () =>
			{
				Hide();
				//if (_fromLogin)
				//{
				//	PopupController.Instance.ShowPopup(PopupType.Authorization);
				//	return;
				//}

				it.Main.PopupController.Instance.ShowPopup(PopupType.MyAvatars);


				//_confirmFormPanel.gameObject.SetActive(true);
				/*_confirmFormPanel.SetEmail(_email);
				_confirmFormPanel.StartTimer(_registrDatetime);*/
			};

			registrationSuccess.OnConfirm = () =>
			{
				//_confirmFormPanel.gameObject.SetActive(true);
				/*_confirmFormPanel.SetEmail(_email);
_confirmFormPanel.StartTimer(_registrDatetime);*/
			};

		}


		public void ShowAuthPopUp()
		{


			Hide();

			it.Main.PopupController.Instance.ShowPopup(PopupType.Authorization);
		}

		public void InitConfirmCodeForm(string email, bool fromLogin = false)
		{
			_fromLogin = fromLogin;
			_email = email;
			_registrationFormPanel.gameObject.SetActive(false);
			//_registrationSuccesPanel.gameObject.SetActive(false);
			_confirmFormPanel.gameObject.SetActive(true);
			_confirmFormPanel.SetEmail(_email);

			if (_fromLogin)
				_confirmFormPanel.FromLogin();

		}


	}
}