using System.Collections;
using UnityEngine;
using TMPro;
using it.Managers;
using it.Network.Rest;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Reflection;
using System;
namespace it.Popups
{
	public class RegistrationConfirm : MonoBehaviour
	{
		public UnityEngine.Events.UnityAction OnRecovery;


		Dictionary<string, string> errorTypes = new Dictionary<string, string>
		{
			["code"] = "errors.forms.noCorrectCode",
			["incorrect_confirmation_code"] = "errors.forms.noCorrectCode"
		};

		[SerializeField] private TMP_InputField[] _inputFields;
		[SerializeField] private TMP_InputField _fullPinField;
		[SerializeField] private string[] _inputValues;
		[SerializeField] private TextMeshProUGUI _errorLabel;
		[SerializeField] private it.UI.Elements.TextButtonUI _buttonResend;
		[SerializeField] private Button _buttonPaste;
		[SerializeField] private TextMeshProUGUI _resetButtonLabel;
		[SerializeField] private Color _activeResentColor;
		[SerializeField] private Color _disableResentColor;
		[SerializeField] private RectTransform _errorRect;

		private string _email;
		private System.DateTime _waitToTime;
		private string _clipboardText = "";

		private void Awake()
		{
			if (_buttonPaste != null)
				_buttonPaste.onClick.AddListener(() => { PasteClipboard(); });
		}

		private void ClearInputForm()
		{
			for (int i = 0; i < _inputFields.Length; i++)
			{
				try
				{
					_inputFields[i].text = "";
					_inputFields[i].caretPosition = 0;
				}
				catch { }
			}

			if (_fullPinField != null)
			{
				try
				{
					_fullPinField.text = "";
				_fullPinField.caretPosition = 0;
				}
				catch { }
			}

		}

		private void Update()
		{
			if (_buttonPaste != null)
			{
				var t = UniClipboard.GetText();
				if (!string.IsNullOrEmpty(t) && t.Length == _inputFields.Length && int.TryParse(t, out _))
				{
						_buttonPaste.gameObject.SetActive(true);
					_clipboardText = t;
				}
				else
				{
						_buttonPaste.gameObject.SetActive(false);
				}
			}
		}
		private void PasteClipboard()
		{
			if (_clipboardText.Length != _inputFields.Length) return;
			for (int i = 0; i < _inputFields.Length; i++)
			{
				_inputFields[i].text = _clipboardText[i].ToString();
			}
		}
		private void OnEnable()
		{
			_errorRect.gameObject.SetActive(false);

			SetDefaultResendButton();
			ClearInputForm();
			_inputValues = new string[_inputFields.Length];

			for (int i = 0; i < _inputFields.Length; i++)
			{
				int index = i;
				_inputFields[index].text = "";
				_inputValues[index] = _inputFields[index].text;

				_inputFields[index].textComponent.fontSizeMax = 20;

				var field = _inputFields[i];

				field.onValueChanged.RemoveAllListeners();
				field.onValueChanged.AddListener((val) =>
				{
					if (_inputValues[index] == "" && val == "")
					{
						var beforeElement = field.FindSelectableOnUp();
						if (beforeElement != null)
						{
							beforeElement.GetComponent<TMP_InputField>().text = "";
							_inputValues[index - 1] = "";
							beforeElement.Select();
						}
					}

					_inputFields[index].textComponent.fontSizeMax = field.text.Length > 0 ? 35 : 20;
					_inputValues[index] = field.text;
					if (field.text.Length > 1)
						field.text = field.text.Substring(0, 1);
					_errorRect.gameObject.SetActive(false);
					if (field.text.Length == 0)
						return;

					var elem = field.FindSelectableOnDown();
					if (elem != null) elem.Select();

				});

			}
			_inputFields[0].onValueChanged.AddListener((val) =>
			{
				if (_clipboardText != "")
				{
					if (_buttonPaste != null)
						PasteClipboard();
				}
			});
		}

		public void StartTimer(System.DateTime dateTime)
		{
			if (dateTime < System.DateTime.Now)
			{
				SetDefaultResendButton();
				return;
			}

			_waitToTime = dateTime;
			StartCoroutine(CoroutineTimer());
		}

		private IEnumerator CoroutineTimer()
		{
			_resetButtonLabel.color = _disableResentColor;
			_buttonResend.StartColor = _disableResentColor;
			_buttonResend.interactable = false;

			while (_waitToTime > System.DateTime.Now)
			{
				_resetButtonLabel.text = $"{I2.Loc.LocalizationManager.GetTermTranslation("popup.registration.code.resendAfter")} {(int)(_waitToTime - System.DateTime.Now).TotalSeconds} sec";
				yield return new WaitForSeconds(1);
			}
			SetDefaultResendButton();
		}

		private void SetDefaultResendButton()
		{
			_buttonResend.interactable = true;
			_buttonResend.StartColor = _activeResentColor;
			_resetButtonLabel.color = _activeResentColor;
			_resetButtonLabel.text = I2.Loc.LocalizationManager.GetTermTranslation("popup.registration.code.resend");
		}

		public void SetEmail(string email)
		{
			_email = email;
		}

		public void ApplyButton()
		{
			string code = "";

			for (int i = 0; i < _inputFields.Length; i++)
			{
				code += _inputFields[i].text;
			}

			if (_fullPinField != null)
				code = _fullPinField.text;

			if (code.Length != _inputFields.Length)
			{
				_errorLabel.text = "Not correct code";
				_errorRect.gameObject.SetActive(true);
				ClearInputForm();
			}

			GetComponentInParent<PopupBase>().Lock(true);
			UserController.Instance.ConfirmCode(_email, code, () =>
			{
				GetComponentInParent<PopupBase>().Lock(false);
				this.GetComponentInParent<PopupBase>().Hide();

				var itm = it.Main.PopupController.Instance.ShowPopup<MyAvatarsPopup>(PopupType.MyAvatars);

				itm.FromRegistration = true;
				OnRecovery?.Invoke();


			}, (error) =>
			{

				ClearInputForm();
				GetComponentInParent<PopupBase>().Lock(false);
				_errorRect.gameObject.SetActive(true);

				try
				{
					//ErrorRest err = it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(error));
					var err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

					_errorLabel.text = errorTypes[err.errors[0].id].Localized();
				}
				catch
				{
					_errorLabel.text = "errors.forms.anyError".Localized();
				}

			});

		}


		public void FromLogin()
		{
			StartTimerResetButton();
		}

		private void StartTimerResetButton()
		{
			StartTimer(System.DateTime.Now.AddSeconds(30));
		}

		/// <summary>
		/// Запрос на новый емаил сщву
		/// </summary>
		public void ResendButton()
		{
			_errorRect.gameObject.SetActive(false);
			UserController.Instance.ResendConfirmationCode(_email, () =>
			{
				StartTimerResetButton();

			}, (error) =>
			{

				GetComponentInParent<PopupBase>().Lock(false);
				_errorRect.gameObject.SetActive(true);
				try
				{
					//ErrorRest err = it.Helpers.ParserHelper.Parse<ErrorRest>(Leguar.TotalJSON.JSON.ParseString(error));
					var err = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(error);

					_errorLabel.text = errorTypes[err.errors[0].id].Localized();
				}
				catch
				{
					_errorLabel.text = "errors.forms.anyError".Localized();
				}
			});

		}

	}

}


