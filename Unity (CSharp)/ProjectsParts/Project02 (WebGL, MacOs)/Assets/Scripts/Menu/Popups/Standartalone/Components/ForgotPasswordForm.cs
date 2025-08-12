using System;
using it.Managers;
using it.Network.Rest;
using it.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace it.Popups
{
	public class ForgotPasswordForm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<string> OnRecovery;

		[SerializeField] private InputFieldValidate _emailInputField;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private GameObject errorGO;

		//public void RecoverButton(){
		//	//OnRecovery?.Invoke();
		//	it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
		//}

		public void Start()
		{
			_emailInputField.InputField.onEndEdit.RemoveAllListeners();
			_emailInputField.InputField.onEndEdit.AddListener((str) =>
			{
				ValidEmailCheck(str);
			});
		}

		private void OnEnable()
		{
			errorGO.SetActive(false);
			_emailInputField.InputField.text = "";
			_emailInputField.InputField.caretPosition = 0;
		}

		public void RecoveryTouch()
		{
			errorGO.gameObject.SetActive(false);
			GetComponentInParent<PopupBase>().Lock(true);
			it.Api.UserApi.RequestRecoveryPassword(_emailInputField.InputField.text, (result) =>
			{
				GetComponentInParent<PopupBase>().Lock(false);
				it.Logger.Log(_emailInputField.InputField.text);
				OnRecovery?.Invoke(_emailInputField.InputField.text);
				_emailInputField.InputField.text = string.Empty;
			}, (error) =>
			{
				GetComponentInParent<PopupBase>().Lock(false);
				ShowError("Invalid email address");
			});
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.Return))
			{
				RecoveryTouch();
			}
		}

		public void ShowError(string errorNext)
		{
			errorGO.SetActive(true);
			_errorLabel.text = errorNext;
		}

		private bool ValidEmailCheck(string str, bool visibleError = true)
		{
			int result = Validator.Email(str);

			if (result == -1)
			{
				_emailInputField.SetInfo(InputFieldValidate.State.None, "");
				return false;
			}

			if (visibleError && str != String.Empty)
			{
				SetEmailState(InputFieldValidate.State.Error, Validator.Email(str));
			}

			return true;
		}


		private void SetEmailState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{
								"main.popup.registration.emailClearError", "main.popup.registration.emailRegexpError"
			};

			_emailInputField.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}


	}
}