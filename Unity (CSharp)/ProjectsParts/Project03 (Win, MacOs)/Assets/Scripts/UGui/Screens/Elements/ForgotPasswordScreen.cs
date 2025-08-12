using Base.Attributes;
using UnityEngine;
using UGui.Screens.Base;
using UGui.Screens.Pages;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UGui.Screens.Elements
{
	[PrefabName(ScreenTypes.ForgotPassword)]
	public class ForgotPasswordScreen : Base.Screen
	{
		[SerializeField] private ForgotPasswordPage _forgotPasswordPage;
		[SerializeField] private ForgotPasswordByEmailPage _forgotByEmailPage;
		[SerializeField] private ForgotPasswordByPhonePage _forgotByPhonePage;
		[SerializeField] private NewPasswordPage _newPasswordPage;

		private string _email;
		private string _phone;
		private string _pin;

		private void OnEnable()
		{
			_email = "";
			_phone = "";
			_pin = "";
			HideAllpages();
			Visible().Forget();
		}

		private void HideAllpages(){
			_forgotPasswordPage.gameObject.SetActive(false);
			_forgotByEmailPage.gameObject.SetActive(false);
			_forgotByPhonePage.gameObject.SetActive(false);
			_newPasswordPage.gameObject.SetActive(false);
		}

		private async UniTask Visible(){

			_forgotPasswordPage.gameObject.SetActive(true);

			_forgotPasswordPage.OnNextEmail = (email) =>
			{
				_email = email;
				_forgotPasswordPage.gameObject.SetActive(false);
				ShowForgotEmailPage();
			};

			_forgotPasswordPage.OnNextPhone = (phone) =>
			{
				_phone = phone;
				_forgotPasswordPage.gameObject.SetActive(false);
				ShowForgotPhonePage();
			};

			await UniTask.WaitUntil(() => _forgotPasswordPage.gameObject.activeInHierarchy);

		}

		private void ShowForgotEmailPage(){

			_forgotByEmailPage.gameObject.SetActive(true);
			_forgotByEmailPage.SetValue(_email);

			_forgotByEmailPage.OnConfirm = (pin)=>{
				_pin = pin;
				_forgotByEmailPage.gameObject.SetActive(false);
				ShowPinPage();
			};
		}

		private void ShowForgotPhonePage()
		{
			_forgotByPhonePage.gameObject.SetActive(true);
			_forgotByPhonePage.SetValue(_phone);

			_forgotByEmailPage.OnConfirm = (pin) => {
				_pin = pin;
				_forgotByEmailPage.gameObject.SetActive(false);
				ShowPinPage();
			};
		}

		private void ShowPinPage(){
			_newPasswordPage.gameObject.SetActive(true);

			Dictionary<string, object> data = new();
			if (!string.IsNullOrEmpty(_email))
				data.Add("email", _email);
			if (!string.IsNullOrEmpty(_phone))
				data.Add("phone", _phone);
			data.Add("code", _pin);

			_newPasswordPage.SetValue(data);

		}

	}
}
