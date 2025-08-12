using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using it.Managers;
using it.UI.Elements;
using DG.Tweening;
using System;
using UnityEngine.Serialization;
 

namespace it.Popups
{
	public class PasswordChangeForm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnConfirm;

		[SerializeField] private InputFieldValidate _passwordCurrentInput;
		[SerializeField] private InputFieldValidate _passwordNewInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private it.UI.Elements.GraphicButtonUI _changeButton;

		private PopupBase _base;

		private void OnEnable()
		{
			_errorLabel.transform.parent.gameObject.SetActive(false);
			_passwordCurrentInput.InputField.text = "";
			_passwordCurrentInput.InputField.caretPosition = 0;
			_passwordCurrentInput.SetInfo(InputFieldValidate.State.None, "");
			_passwordNewInput.InputField.text = "";
			_passwordNewInput.InputField.caretPosition = 0;
			_passwordNewInput.SetInfo(InputFieldValidate.State.None, "");

		}

		private void Start()
		{
			_passwordCurrentInput.InputField.onEndEdit.RemoveAllListeners();
			_passwordCurrentInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidPasswordCheck(_passwordCurrentInput, str);

			});
			_passwordNewInput.InputField.onEndEdit.RemoveAllListeners();
			_passwordNewInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidPasswordCheck(_passwordNewInput, str);

			});


		}

		private bool FormCheck()
		{

			string confirmValue = _passwordCurrentInput.InputField.text;
			string newPassword = _passwordNewInput.InputField.text;

			//if (newPassword != confirmValue)
			//{
			//	_errorLabel.text = "Passwords do not match";
			//	_errorLabel.transform.parent.gameObject.SetActive(true);
			//	return false;
			//}

			int validateCurrent = Validator.Password(confirmValue);

			if (validateCurrent != -1)
			{
				switch (validateCurrent)
				{
					case 0:
						{
							_passwordCurrentInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.empty".Localized());
							break;
						}
					case 1:
						{
							_passwordCurrentInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.lenght_8_20".Localized());
							break;
						}
					case 2:
					default:
						{
							_passwordCurrentInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.noCorrect".Localized());
							break;
						}
				}
			}
			else
			{
				_passwordCurrentInput.SetInfo(InputFieldValidate.State.None, "");

			}


			int validateNew = Validator.Password(newPassword);

			if (validateNew != -1)
			{
				switch (validateNew)
				{
					case 0:
						{
							_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.empty".Localized());
							break;
						}
					case 1:
						{
							_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.lenght_8_20".Localized());
							break;
						}
					case 2:
					default:
						{
							_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.noCorrect".Localized());
							break;
						}
				}
			}
			else
			{
				_passwordNewInput.SetInfo(InputFieldValidate.State.None, "errors.forms.noCorrect".Localized());
			}




			return true;
		}

		private int ValidPasswordCheck(InputFieldValidate inputFieldValidate, string str, bool visibleError = true)
		{
			int result = Validator.Password(str);

			if (result == -1)
			{
				inputFieldValidate.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && str != String.Empty)
			{
				SetPassword(inputFieldValidate, InputFieldValidate.State.Error, result);
			}

			return result;
		}

		private void SetPassword(InputFieldValidate fieldValidate, InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{
				"errors.forms.empty"
				, "errors.forms.lenght_8_20"
				, "errors.forms.noCorrect"
			};

			fieldValidate.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		public void ConfirmButtonTouch()
		{

			if (!FormCheck()) return;

			string confirmValue = _passwordCurrentInput.InputField.text;
			string newPassword = _passwordNewInput.InputField.text;

			_errorLabel.transform.parent.gameObject.SetActive(false);


			if (_base == null)
				_base = GetComponentInParent<PopupBase>();

			_base.Lock(true);

			it.Logger.Log(confirmValue + newPassword);

			it.Api.UserApi.ChangePassword(confirmValue, newPassword, (result) =>
			{
				it.Logger.Log(result.IsSuccess);
				_base.Lock(false);
				if (result.IsSuccess)
				{
					//UserController.Instance.GetUserData();
					OnConfirm?.Invoke();
				}
				else
				{
					_errorLabel.text = "errors.forms.noCorrectDataForm".Localized();
					_errorLabel.transform.parent.gameObject.SetActive(true);
				}

			});

		}

	}
}
