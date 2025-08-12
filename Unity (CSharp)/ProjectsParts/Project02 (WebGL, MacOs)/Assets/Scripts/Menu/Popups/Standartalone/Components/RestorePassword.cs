using UnityEngine;
using TMPro;
using it.UI.Elements;
using System;
using it.Popups;
 

public class RestorePassword : MonoBehaviour
{

	public UnityEngine.Events.UnityAction OnConfirm;

	[SerializeField] private InputFieldValidate _passwordNewInpitFirst;
	[SerializeField] private InputFieldValidate _passwordNewInput;
	[SerializeField] private TextMeshProUGUI _errorLabel;
	[SerializeField] private GameObject erroGO;
	[SerializeField] private it.UI.Elements.GraphicButtonUI _changeButton;

	private PopupBase _base;
	private string _email;
	private string _token;

	private void OnEnable()
	{
		erroGO.SetActive(false);
		_passwordNewInpitFirst.InputField.text = "";
		_passwordNewInpitFirst.InputField.caretPosition = 0;
		_passwordNewInpitFirst.SetInfo(InputFieldValidate.State.None, "");
		_passwordNewInput.InputField.text = "";
		_passwordNewInput.InputField.caretPosition = 0;
		_passwordNewInput.SetInfo(InputFieldValidate.State.None, "");

	}

	private void Start()
	{
		_passwordNewInpitFirst.InputField.onEndEdit.RemoveAllListeners();
		_passwordNewInpitFirst.InputField.onEndEdit.AddListener((str) =>
		{
			ValidPasswordCheck(_passwordNewInpitFirst, str);
		});
		_passwordNewInput.InputField.onEndEdit.RemoveAllListeners();
		_passwordNewInput.InputField.onEndEdit.AddListener((str) =>
		{
			ValidPasswordCheck(_passwordNewInput, str);
		});
	}

	private bool FormCheck()
	{

		string confirmValue = _passwordNewInpitFirst.InputField.text;
		string newPassword = _passwordNewInput.InputField.text;

		if (newPassword != confirmValue)
		{
			_errorLabel.text = "errors.forms.passwordNoMatch".Localized();
			erroGO.SetActive(true);
			return false;
		}

		int validateCurrent = Validator.Password(confirmValue);

		if (validateCurrent != -1)
		{

			switch (validateCurrent)
			{
				case 0:
					{
						_passwordNewInpitFirst.SetInfo(InputFieldValidate.State.Error, "errors.forms.empty".Localized());
						return false;
					}
				case 1:
					{
						_passwordNewInpitFirst.SetInfo(InputFieldValidate.State.Error, "errors.forms.lenght_8_20".Localized());
						return false;
					}
				case 2:
				default:
					{
						_passwordNewInpitFirst.SetInfo(InputFieldValidate.State.Error, "errors.forms.noCorrect".Localized());
						return false;
					}
			}
		}

		int validateNew = Validator.Password(newPassword);

		if (validateNew != -1)
		{

			switch (validateNew)
			{
				case 0:
					{
						_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.empty".Localized());
						return false;
					}
				case 1:
					{
						_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.lenght_8_20".Localized());
						return false;
					}
				case 2:
				default:
					{
						_passwordNewInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.noCorrect".Localized());
						return false;
					}

			}
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
				,"errors.forms.lenght_8_20"
				,"errors.forms.noCorrect"
		};

		fieldValidate.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
	}

	public void SetData(string email, string token)
	{
		_email = email;
		_token = token;
	}

	public void ConfirmButtonTouch()
	{
		erroGO.SetActive(false);

		if (_passwordNewInpitFirst.InputField.text != _passwordNewInput.InputField.text)
		{
			erroGO.SetActive(true);
			_errorLabel.text = "errors.forms.passwordNoMatch".Localized();
		}

		if (!FormCheck()) return;

		string confirmValue = _passwordNewInpitFirst.InputField.text;
		string newPassword = _passwordNewInput.InputField.text;


		if (_base == null)
			_base = GetComponentInParent<PopupBase>();

		_base.Lock(true);

		it.Api.UserApi.RestorePassword(_email, _token, newPassword, (result) =>
				{

					_base.Lock(false);
					if (result.IsSuccess)
					{

						erroGO.SetActive(false);
						//UserController.Instance.GetUserData();
						OnConfirm?.Invoke();
					}

				});

	}

}
