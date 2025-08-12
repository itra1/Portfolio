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
	public class RegistrationForm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction<string> OnRegistered;

		Dictionary<string, string> errorTypes = new Dictionary<string, string>
		{
			["user_already_registered"] = "errors.forms.userRlreadyRegistered",
			["nickname_taken"] = "errors.forms.nicknameToken",
			["invalid_phone"] = "errors.forms.invalidPhone",
		};

		[SerializeField] private I2.Loc.Localize _localizationYead18;
		[SerializeField] private InputDropDown _regionDropDown;
		[SerializeField] private InputFieldValidate _nicknameInput;
		[SerializeField] private InputFieldValidate _emailInput;
		[SerializeField] private InputFieldValidate _phoneInput;
		[SerializeField] private InputFieldValidate _passwordInput;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private GraphicButtonUI registrButton;
		[SerializeField] private TMP_InputField _bonusInputField;
		[SerializeField] private Toggle _rulesToggle;
		[SerializeField] private Sprite[] _continueButtonsSprite;

		private List<Region> _regionList = new List<Region>();

		public IEnumerator Start()
		{

		#if UNITY_IOS

		if(_localizationYead18 != null){
				_localizationYead18.SetTerm("popup.registration.rules_iOS");
		}

		#endif

			_regionDropDown.onChangeValue.RemoveAllListeners();
			_regionDropDown.onChangeValue.AddListener((val) =>
			{
				CheckAllFill();
			});
			_nicknameInput.InputField.onEndEdit.RemoveAllListeners();
			_nicknameInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidNicknameCheck(str);
				CheckAllFill();
			});
			_emailInput.InputField.onEndEdit.RemoveAllListeners();
			_emailInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidEmailCheck(str);
				CheckAllFill();
			});
			_passwordInput.InputField.onEndEdit.RemoveAllListeners();
			_passwordInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidPasswordCheck(str);
				CheckAllFill();
			});
			_phoneInput.InputField.onEndEdit.RemoveAllListeners();
			_phoneInput.InputField.onEndEdit.AddListener((str) =>
			{
				ValidPhoneNumber(str);
				CheckAllFill();
			});

			_passwordInput.InputField.caretPosition = 0;
			_emailInput.InputField.caretPosition = 0;
			_nicknameInput.InputField.caretPosition = 0;

			_rulesToggle.onValueChanged.RemoveAllListeners();
			_rulesToggle.onValueChanged.AddListener((res) =>
			{
				CheckAllFill();
			});

			ClearForm();
			_regionDropDown.Options.Clear();

			_regionDropDown.enabled = false;
			yield return new WaitUntil(() => HelpMaterialController.Instance.RegionList.Count > 0);
			_regionDropDown.enabled = true;
			for (int i = 0; i < HelpMaterialController.Instance.RegionList.Count; i++)
			{
				if (HelpMaterialController.Instance.RegionList[i].can_register_with)
				{
					_regionList.Add(HelpMaterialController.Instance.RegionList[i]);
					_regionDropDown.Options.Add(HelpMaterialController.Instance.RegionList[i].title);
				}
			}
			registrButton.interactable = false;
		}

		private void OnEnable()
		{
			ClearForm();
		}

		private void Update()
		{
			if (Input.GetKey(KeyCode.Return))
			{
				ContinueButton();
			}
		}

		private void ClearForm()
		{
			_nicknameInput.InputField.text = "";
			_emailInput.InputField.text = "";
			_passwordInput.InputField.text = "";
			_passwordInput.InputField.text = "";
			_bonusInputField.text = "";

			_passwordInput.InputField.caretPosition = 0;
			_emailInput.InputField.caretPosition = 0;
			_nicknameInput.InputField.caretPosition = 0;
			_passwordInput.InputField.caretPosition = 0;
			_bonusInputField.caretPosition = 0;

			_rulesToggle.isOn = false;
			_regionDropDown.Clear();
			SetNicknameState(InputFieldValidate.State.None, 1);
			SetEmailState(InputFieldValidate.State.None, 1);
			SetPasswordState(InputFieldValidate.State.None, 1);

			//if (PlayerPrefs.HasKey("promo"))
			//{
			//	_bonusInputField.text = PlayerPrefs.GetString("promo", "");
			//}

			//_nicknameInput.SetInfo(InputFieldValidate.State.Info, "");
			//_emailInput.SetInfo(InputFieldValidate.State.Info, "");
			//_passwordInput.SetInfo(InputFieldValidate.State.Info, "");

			//_nicknameValidatorHelper.gameObject.SetActive(false);
			//_emailValidatorHelper.gameObject.SetActive(false);
			//_passwordValidatorHelper.gameObject.SetActive(false);
			_errorLabel.gameObject.SetActive(false);
		}

		public void ContinueButton()
		{
			if (!CheckAllFill(true))
				return;

			if (_regionDropDown.Value < 0)
				return;

			GetComponentInParent<PopupBase>().Lock(true);


			//if (PlayerPrefs.HasKey("promo"))
			//{
			//	_bonusInputField.text = PlayerPrefs.GetString("promo", "");
			//}

			it.Api.UserApi.Registration(
		_emailInput.InputField.text,
		_passwordInput.InputField.text,
		_nicknameInput.InputField.text,
		"+" + _phoneInput.InputField.text,
		_regionList[_regionDropDown.Value].id,
		_bonusInputField.text,
		PlayerPrefs.GetString("promo", ""),
		(result) =>
		{
			if (result.IsSuccess)
			{
				if (PlayerPrefs.HasKey("promo"))
					PlayerPrefs.DeleteKey("promo");

				GetComponentInParent<PopupBase>().Lock(false);
				//UserController.Instance.ClearStag();
				OnRegistered?.Invoke(_emailInput.InputField.text);
				return;

			}

			GetComponentInParent<PopupBase>().Lock(false);

			try
			{
				//ErrorRest err =	it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(result.ErrorMessage));
				ErrorResponse err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(result.ErrorMessage);

				switch (err.errors[0].id)
				{
					case "user_already_registered":
						{
							_emailInput.SetInfo(InputFieldValidate.State.Error, errorTypes[err.errors[0].id].Localized());
							break;
						}
					case "nickname_taken":
						{
							_nicknameInput.SetInfo(InputFieldValidate.State.Error, errorTypes[err.errors[0].id].Localized());
							break;
						}
					case "phone":
						{
							if (err.errors[0].code == "validation_error")
							{
								_phoneInput.SetInfo(InputFieldValidate.State.Error, "errors.forms.invalidPhone".Localized());
							}

							break;
						}
					default:
						{
							_errorLabel.gameObject.SetActive(true);
							_errorLabel.text = "errors.forms.serverError".Localized();
							break;
						}
				}

				//_errorLabel.text = errorTypes[err.Errors[0].Id];
			}
			catch
			{
				_errorLabel.gameObject.SetActive(true);
				_errorLabel.text = "errors.forms.serverError".Localized();
			}

			// it.Main.PopupController.Instance.ShowPopup(PopupType.RegistrationSuccess);
		});

		}

		private int ValidNicknameCheck(string str, bool visibleError = false)
		{
			int result = Validator.Nickname(str);

			if (result == -1)
			{
				_nicknameInput.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && !string.IsNullOrEmpty(str))
			{
				SetNicknameState(InputFieldValidate.State.Error, result);
			}

			return result;
		}

		private void SetNicknameState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{"errors.forms.empty"
			,"errors.forms.lenght_3_15"
			,"errors.forms.noCorrect"
			};

			_nicknameInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		private int ValidEmailCheck(string str, bool visibleError = false)
		{
			int result = Validator.Email(str);

			if (result == -1)
			{
				_emailInput.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && !string.IsNullOrEmpty(str))
			{
				SetEmailState(InputFieldValidate.State.Error, result);
			}

			return result;
		}

		private int ValidPhoneNumber(string str, bool visibleError = false)
		{
			int result = Validator.PhoneNumber(str);

			if (result == -1)
			{
				_phoneInput.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && !string.IsNullOrEmpty(str))
			{
				SetPhoneState(InputFieldValidate.State.Error, result);
			}

			return result;
		}

		private void SetEmailState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{"errors.forms.empty"
			,"errors.forms.noCorrect"
			};

			_emailInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		private void SetPhoneState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{"errors.forms.empty"
			,"errors.forms.noCorrect"
			};

			_phoneInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		private int ValidPasswordCheck(string str, bool visibleError = false)
		{
			int result = Validator.Password(str);

			if (result == -1)
			{
				_passwordInput.SetInfo(InputFieldValidate.State.None, "");
				return -1;
			}

			if (visibleError && !string.IsNullOrEmpty(str))
			{
				SetPasswordState(InputFieldValidate.State.Error, result);
			}

			return result;
		}

		private void SetPasswordState(InputFieldValidate.State state, int indexMessage)
		{
			string[] lang = new string[]
			{"errors.forms.empty"
			,"errors.forms.lenght_8_20"
			,"errors.forms.noCorrect"
			};

			_passwordInput.SetInfo(state, I2.Loc.LocalizationManager.GetTranslation(lang[indexMessage]));
		}

		private void ShowValidError(RectTransform errorRect, string error)
		{
			errorRect.gameObject.SetActive(true);
			TextMeshProUGUI tLabel = errorRect.GetComponentInChildren<TextMeshProUGUI>();
			tLabel.text = error;
			errorRect.sizeDelta = new Vector2(errorRect.sizeDelta.x, tLabel.preferredHeight + 20);
		}

		private bool CheckAllFill(bool visibleError = true)
		{
			bool validForm = true;
			if (_regionDropDown.Value == -1)
			{
				validForm = false;
			}

			var res = ValidNicknameCheck(_nicknameInput.InputField.text, visibleError);
			if (res != -1)
			{
				validForm = false;
			}

			res = ValidEmailCheck(_emailInput.InputField.text, visibleError);
			if (res != -1)
			{
				validForm = false;
			}

			res = ValidPhoneNumber(_phoneInput.InputField.text, visibleError);
			if (res != -1)
			{
				validForm = false;
			}

			res = ValidPasswordCheck(_passwordInput.InputField.text, visibleError);
			if (res != -1)
			{
				validForm = false;
			}

			if (!_rulesToggle.isOn)
			{
				validForm = false;
			}

			registrButton.interactable = validForm;
			return validForm;
		}


		public void TermsAndConditionsTouch()
		{
			Garilla.LinkManager.OpenUrl("termAndConditions");
		}

		public void PrivacyPolicyTouch()
		{
			Garilla.LinkManager.OpenUrl("privacyPolicy");
		}
	}
}