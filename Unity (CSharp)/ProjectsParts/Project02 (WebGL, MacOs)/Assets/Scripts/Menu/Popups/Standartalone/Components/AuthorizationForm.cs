using System;
using it.Network.Rest;
using it.UI.Elements;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using it.Main;

namespace it.Popups
{
	public class AuthorizationForm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnLoginConfirm;
		public UnityEngine.Events.UnityAction OnSign;
		public UnityEngine.Events.UnityAction OnForgot;

		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private RectTransform _errorImage;
		[SerializeField] private InputFieldValidate _emailInput;
		[SerializeField] private InputFieldValidate _passwordInput;
		[SerializeField] private Toggle _rememberToggle;

		Dictionary<string, string> errorTypes = new Dictionary<string, string>
		{
			["emptyFields"] = "errors.forms.noCorrectDataForm",
			["notCorrectEmail"] = "errors.forms.invalidEmailOrPassword",
			["user_not_found"] = "errors.forms.invalidEmailOrPassword"
		};

		public static string errorType;
		private bool _authProcess;

		public void Start()
		{
			_emailInput.InputField.onEndEdit.RemoveAllListeners();
			_emailInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidEmailCheck(str);
			});
			_passwordInput.InputField.onEndEdit.RemoveAllListeners();
			_passwordInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidPassword(str);
			});
		}

		public void OnEnable()
		{
			_emailInput.InputField.text = "";
			_passwordInput.InputField.text = "";
			_emailInput.InputField.caretPosition = 0;
			_passwordInput.InputField.caretPosition = 0;
			_errorImage.gameObject.SetActive(false);

			SetEmailState(InputFieldValidate.State.None, 1);
			SetPasswordState(InputFieldValidate.State.None, 1);
		}

		private void Update()
		{
			//if (Input.GetButtonUp("Submit"))
			//{
			//	if (EventSystem.current.currentSelectedGameObject != null)
			//	{
			//		Selectable s = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
			//		if (s != null && s == _passwordInput.InputField)
			//		{
			//			LoginButton();
			//		}
			//	}
			//}

			if (Input.GetKey(KeyCode.Return))
			{
				LoginButton();
			}
		}

		public void LoginButton()
		{
			if (_authProcess) return;
			_authProcess = true;

			_errorImage.gameObject.SetActive(false);
			string emailValue = _emailInput.InputField.text;
			string passwordValue = _passwordInput.InputField.text;
			emailValue = Regex.Replace(emailValue, @"[ \r\n\t]", "");
			passwordValue = Regex.Replace(passwordValue, @"[ \r\n\t]", "");

			GetComponentInParent<AuthorizationPopup>().Lock(true);

			UserController.Instance.Login(emailValue, passwordValue, _rememberToggle.isOn, () =>
			{
				_authProcess = false;
				OnLoginConfirm?.Invoke();
			}, (error) =>
			{
				_authProcess = false;
				GetComponentInParent<AuthorizationPopup>().Lock(false);

				try
				{
					//ErrorRest err = it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(error));
					var err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

					if (errorTypes.ContainsKey(err.errors[0].id))
					{
						_errorLabel.text = errorTypes[err.errors[0].id].Localized();
						_errorImage.gameObject.SetActive(true);
						return;
					}
					switch (err.errors[0].id)
					{
						case "email_not_confirmed":
							{

								GetComponentInParent<AuthorizationPopup>().Hide();

								var popup = PopupController.Instance.ShowPopup<RegistrationPopup>(PopupType.Registration);

								popup.InitConfirmCodeForm(emailValue, true);

								//_errorImage.gameObject.SetActive(true);
								//_errorLabel.text = "errors.forms.invalidEmailOrPassword".Localized();

								return;
							}
						case "user_not_found":
						case "password":
						default:
							{
								//_emailInput.SetInfo(InputFieldValidate.State.Error, errorTypes[err.Errors[0].Id]);
								_errorImage.gameObject.SetActive(true);
								_errorLabel.text = "errors.forms.invalidEmailOrPassword".Localized();
								return;
							}
					}
				}
				catch
				{
					_errorImage.gameObject.SetActive(true);
					_errorLabel.text = "errors.forms.invalidEmailOrPassword".Localized();
				}
			});
		}

		private void SetEmailState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{
								"main.popup.registration.emailClearError", "main.popup.registration.emailRegexpError"
			};

			_emailInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		private void SetPasswordState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{
								"main.popup.registration.passwordClearError", "main.popup.registration.passwordLenghtError",
								"main.popup.registration.passwordRegexpError"
			};

			_passwordInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		public void SingInButton()
		{
			OnSign?.Invoke();
			// it.Main.PopupController.Instance.ShowPopup(PopupType.Registration);
			// Hide();
		}

		public void ForgotPasswordButton()
		{
			OnForgot?.Invoke();
			//it.Main.PopupController.Instance.ShowPopup(PopupType.Develop);
			//Hide();
		}

		private void OutputError()
		{
			_errorLabel.gameObject.SetActive(true);
			_errorLabel.text = errorTypes[errorType];
		}


		private int ValidEmailCheck(string str, bool visibleError = true)
		{
			int result = Validator.Email(str);

			if (result == -1)
			{

				_emailInput.SetInfo(InputFieldValidate.State.None, "");

				return -1;
			}

			if (visibleError && str != String.Empty)
			{


				SetEmailState(InputFieldValidate.State.Error, Validator.Email(str));
			}

			return result;
		}

		private int ValidPassword(string str, bool visibleError = true)
		{
			int result = Validator.Password(str);
			if (result == -1)
			{
				_passwordInput.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && str != String.Empty)
			{
				SetPasswordState(InputFieldValidate.State.Error, Validator.Password(str));
			}

			return result;
		}

	}


}